using Silk.NET.Direct3D12;
using Silk.NET.DXGI;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace TestD3d12;

public unsafe class App(IWindow window, GraphicsContext ctx, HwndSwapChain swap_chain)
{
    public void OnLoad()
    {
        D3D12MA.AllocationDesc allocation_desc = new()
        {
            Flags = D3D12MA.AllocationFlags.None,
            HeapType = HeapType.Default,
            ExtraHeapFlags = HeapFlags.None,
            CustomPool = null,
            pPrivateData = null
        };
        ResourceDesc1 resource_desc = new()
        {
            Dimension = ResourceDimension.Texture2D,
            Width = 1024,
            Height = 1024,
            DepthOrArraySize = 1,
            MipLevels = 1,
            Format = Format.FormatR8G8B8A8Unorm,
            SampleDesc = new(1, 0),
            Layout = TextureLayout.LayoutUnknown,
            Flags = ResourceFlags.None,
        };
        D3D12MA.Allocation* p_allocation;
        ctx.Allocator.Handle->CreateResource3(
            &allocation_desc, &resource_desc,
            BarrierLayout.CopyDest, null, 0, null,
            &p_allocation, null, null
        ).TryThrowHResult();
    }

    public void OnUpdate(double delta_time) { }

    public void OnRender(double delta_time)
    {
        ctx.CommandList.Barrier([
            new TextureBarrier(
                BarrierSync.None, BarrierSync.RenderTarget,
                BarrierAccess.NoAccess, BarrierAccess.RenderTarget,
                BarrierLayout.Common, BarrierLayout.RenderTarget,
                swap_chain.Ptr
            )
        ]);
        ctx.CommandList.ClearRenderTargetView(swap_chain.CurrentRtv, 1);
        ctx.CommandList.Barrier([
            new TextureBarrier(
                BarrierSync.RenderTarget, BarrierSync.None,
                BarrierAccess.RenderTarget, BarrierAccess.NoAccess,
                BarrierLayout.RenderTarget, BarrierLayout.Common,
                swap_chain.Ptr
            )
        ]);
    }

    public void OnKeyDown(IKeyboard keyboard, Key key, int key_code) { }
}
