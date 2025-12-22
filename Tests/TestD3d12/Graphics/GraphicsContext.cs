using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Coplt.Dropping;
using Serilog;
using Serilog.Events;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D12;
using Silk.NET.DXGI;

namespace TestD3d12;

[Dropping(Unmanaged = true)]
public unsafe partial class GraphicsContext
{
    #region Consts

    public const int FrameCount = 3;

    private static readonly RootSignatureFlags[] s_root_signature_flag_variants =
    [
        RootSignatureFlags.CbvSrvUavHeapDirectlyIndexed | RootSignatureFlags.SamplerHeapDirectlyIndexed,
        RootSignatureFlags.CbvSrvUavHeapDirectlyIndexed | RootSignatureFlags.SamplerHeapDirectlyIndexed |
        RootSignatureFlags.AllowInputAssemblerInputLayout
    ];

    #endregion

    #region Props Fields

    public DXGI Dxgi { get; }
    public D3D12 D3d12 { get; }
    public bool DebugEnabled { get; }

    [Drop]
    private ComPtr<ID3D12Debug> m_debug_controller;
    [Drop]
    private ComPtr<IDXGIFactory6> m_factory;
    [Drop]
    private ComPtr<IDXGIAdapter1> m_adapter;
    [Drop]
    private ComPtr<ID3D12Device10> m_device;
    [Drop]
    private ComPtr<ID3D12InfoQueue1> m_info_queue;
    [Drop]
    private ComPtr<ID3D12CommandQueue> m_queue;
    [Drop]
    private ComPtr<ID3D12Fence> m_fence;

    private readonly Queue<IGpuRecyclable> m_recycle_queue = new();
    private readonly Lock m_recycle_lock = new();

    [Drop]
    private InlineArray3<ComPtr<ID3D12CommandAllocator>> m_cmd_allocator;
    [Drop]
    private CommandList m_main_list;

    private EventWaitHandle m_event;
    private InlineArray3<ulong> m_fence_values = default;
    private ulong fence_value;
    private int m_cur_frame;
    private uint m_callback_cookie;

    public ref readonly ComPtr<IDXGIFactory6> Factory => ref m_factory;
    public ref readonly ComPtr<IDXGIAdapter1> Adapter => ref m_adapter;
    public ref readonly ComPtr<ID3D12Device10> Device => ref m_device;

    public ref readonly ComPtr<ID3D12CommandQueue> Queue => ref m_queue;
    public ref readonly ComPtr<ID3D12Fence> Fence => ref m_fence;

    public ReadOnlySpan<ComPtr<ID3D12CommandAllocator>> CommandAllocator => m_cmd_allocator;
    public CommandList CommandList => m_main_list;

    public int CurrentFrame => m_cur_frame;

    #endregion

    #region Ctor

    public GraphicsContext(DXGI dxgi, D3D12 d3d12, bool debug)
    {
        Dxgi = dxgi;
        D3d12 = d3d12;

        #region create event

        m_event = new(false, EventResetMode.AutoReset);

        #endregion

        #region create dx12

        var dxgi_flags = 0u;
        if (debug)
        {
            if (((HResult)D3d12.GetDebugInterface(out m_debug_controller)).IsSuccess)
            {
                m_debug_controller.EnableDebugLayer();
                dxgi_flags |= DXGI.CreateFactoryDebug;
                DebugEnabled = true;
            }

            if (((HResult)m_debug_controller.QueryInterface(out ComPtr<ID3D12Debug5> debug5)).IsSuccess)
            {
                debug5.EnableDebugLayer();
                debug5.Dispose();
            }
        }

        Dxgi.CreateDXGIFactory2(dxgi_flags, out m_factory).TryThrowHResult();
        m_factory.EnumAdapterByGpuPreference(0, GpuPreference.HighPerformance, out m_adapter).TryThrowHResult();

        D3d12.CreateDevice(m_adapter, D3DFeatureLevel.Level121, out m_device).TryThrowHResult();
        if (DebugEnabled)
        {
            m_device.SetName(in "Main Device".AsSpan()[0]).TryThrowHResult();
            if (m_device.QueryInterface(out m_info_queue).AsHResult().IsSuccess)
            {
                uint cookie = 0;
                if (m_info_queue.Handle->RegisterMessageCallback(
                        (delegate* unmanaged[Cdecl]<MessageCategory, MessageSeverity, MessageID, byte*, void*, void>)&DebugCallback,
                        MessageCallbackFlags.FlagNone,
                        null,
                        &cookie
                    ).AsHResult().IsSuccess)
                {
                    m_callback_cookie = cookie;
                }
                else
                {
                    Log.Warning("Failed to RegisterMessageCallback");
                }
            }
        }

        #endregion

        #region create queue

        CommandQueueDesc queue_desc = new()
        {
            Type = CommandListType.Direct,
            Priority = 0,
            Flags = CommandQueueFlags.None,
            NodeMask = 0
        };
        m_device.CreateCommandQueue(&queue_desc, out m_queue).TryThrowHResult();
        if (DebugEnabled) m_queue.SetName(in "Main Queue".AsSpan()[0]).TryThrowHResult();

        #endregion

        #region create fence

        m_device.CreateFence(0u, FenceFlags.None, out m_fence).TryThrowHResult();
        if (DebugEnabled) m_fence.SetName(in "Main Fence".AsSpan()[0]).TryThrowHResult();

        #endregion

        #region create command

        for (var i = 0; i < FrameCount; i++)
        {
            ref var ca = ref m_cmd_allocator[i];
            m_device.Handle->CreateCommandAllocator(CommandListType.Direct, out ca).TryThrowHResult();
        }

        m_device.Handle->CreateCommandList(0, CommandListType.Direct, m_cmd_allocator[0], default(ComPtr<ID3D12PipelineState>),
                out ComPtr<ID3D12GraphicsCommandList7> cmd_list)
            .TryThrowHResult();
        m_main_list = new(this, cmd_list, CommandListType.Direct) { m_main = true };

        #endregion
    }

    #endregion

    #region Drop

    [Drop(Order = -1000)]
    private void WaitAll()
    {
        Wait(Signal());
    }

    #endregion

    #region DebugCallback

    [Drop(Order = -1)]
    private void UnRegDebugCallback()
    {
        if (m_info_queue.Handle == null) return;
        m_info_queue.Handle->UnregisterMessageCallback(m_callback_cookie);
        m_callback_cookie = 0;
    }

    private enum DxMessageCategory
    {
        ApplicationDefined = 0,
        Miscellaneous = 1,
        Initialization = 2,
        Cleanup = 3,
        Compilation = 4,
        StateCreation = 5,
        StateSetting = 6,
        StateGetting = 7,
        ResourceManipulation = 8,
        Execution = 9,
        Shader = 10,
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void DebugCallback(MessageCategory Category, MessageSeverity Severity, MessageID id, byte* pDescription, void* pContext)
    {
        try
        {
            var level = Severity switch
            {
                MessageSeverity.Corruption => LogEventLevel.Fatal,
                MessageSeverity.Error => LogEventLevel.Error,
                MessageSeverity.Warning => LogEventLevel.Warning,
                MessageSeverity.Info => LogEventLevel.Information,
                MessageSeverity.Message => LogEventLevel.Debug,
                _ => LogEventLevel.Error
            };
            if (!Log.IsEnabled(level)) return;
            var cat = Category switch
            {
                MessageCategory.ApplicationDefined => DxMessageCategory.ApplicationDefined,
                MessageCategory.Miscellaneous => DxMessageCategory.Miscellaneous,
                MessageCategory.Initialization => DxMessageCategory.Initialization,
                MessageCategory.Cleanup => DxMessageCategory.Cleanup,
                MessageCategory.Compilation => DxMessageCategory.Compilation,
                MessageCategory.StateCreation => DxMessageCategory.StateCreation,
                MessageCategory.StateSetting => DxMessageCategory.StateSetting,
                MessageCategory.StateGetting => DxMessageCategory.StateGetting,
                MessageCategory.ResourceManipulation => DxMessageCategory.ResourceManipulation,
                MessageCategory.Execution => DxMessageCategory.Execution,
                MessageCategory.Shader => DxMessageCategory.Shader,
                _ => (DxMessageCategory)Category,
            };
            var msg = new string((sbyte*)pDescription);
            Log.Write(level, "[{Category}] {Msg}", cat, msg);
        }
        catch (Exception e)
        {
            try
            {
                Log.Error(e, "Unhandled exception");
            }
            catch
            {
                // ignored
            }
        }
    }

    #endregion

    #region Fence

    public ulong AllocSignal() => Interlocked.Increment(ref fence_value);

    public ulong Signal()
    {
        var value = AllocSignal();
        m_queue.Signal(m_fence.Handle, value).TryThrowHResult();
        return value;
    }

    /// <summary>
    /// Wait On Gpu
    /// </summary>
    public void Wait(ulong value) => m_queue.Wait(m_fence.Handle, value).TryThrowHResult();

    /// <summary>
    /// Wait On Cpu
    /// </summary>
    public void Wait(ulong value, EventWaitHandle handle) => m_fence.Wait(value, handle);

    /// <summary>
    /// Wait On Cpu
    /// </summary>
    public ValueTask WaitAsync(ulong value, EventWaitHandle handle) => m_fence.WaitAsync(value, handle);

    #endregion

    #region Submit

    public void Submit()
    {
        SubmitNotEnd();
        EndFrame();
    }

    public void SubmitNotEnd()
    {
        m_main_list.Raw->Close();
        var list = (ID3D12CommandList*)m_main_list.Raw;
        m_queue.Handle->ExecuteCommandLists(1, &list);
    }

    public void EndFrame()
    {
        m_fence_values[m_cur_frame] = Signal();
    }

    public void ReadyNextFrame()
    {
        m_cur_frame++;
        if (m_cur_frame >= FrameCount) m_cur_frame = 0;
        var value = m_fence_values[m_cur_frame];
        Wait(value, m_event);
        m_cmd_allocator[m_cur_frame].Handle->Reset().TryThrowHResult();
        m_main_list.Raw->Reset(m_cmd_allocator[m_cur_frame].Handle, null).TryThrowHResult();
        Recycle();
    }

    public void ReadyNextFrameNoWait()
    {
        m_cur_frame++;
        if (m_cur_frame >= FrameCount) m_cur_frame = 0;
        m_cmd_allocator[m_cur_frame].Handle->Reset().TryThrowHResult();
        m_main_list.Raw->Reset(m_cmd_allocator[m_cur_frame].Handle, null).TryThrowHResult();
        Recycle();
    }

    #endregion

    #region Recycle

    public void RegRecycle(IGpuRecyclable item)
    {
        using var _ = m_recycle_lock.EnterScope();
        m_recycle_queue.Enqueue(item);
    }

    private void Recycle()
    {
        using var _ = m_recycle_lock.EnterScope();
        re:
        if (m_recycle_queue.TryPeek(out var item))
        {
            if (item.CurrentFrame != m_cur_frame) return;
            item.Recycle();
            m_recycle_queue.Dequeue();
            goto re;
        }
    }

    #endregion
}
