using System.Runtime.CompilerServices;
using Coplt.Dropping;
using Coplt.Mathematics;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D12;

namespace TestD3d12;

[Dropping(Unmanaged = true)]
public unsafe partial class CommandList(GraphicsContext Context, ComPtr<ID3D12GraphicsCommandList7> list, CommandListType type)
{
    #region Fields

    public GraphicsContext Context { get; } = Context;
    public CommandListType Type { get; } = type;

    [Drop]
    private ComPtr<ID3D12GraphicsCommandList7> list = list;

    // private ShaderPipeline? m_current_pipeline;

    internal bool m_main;

    #endregion

    #region Properties

    public ref readonly ComPtr<ID3D12GraphicsCommandList7> List => ref list;
    public ID3D12GraphicsCommandList7* Raw => list.Handle;

    #endregion

    #region Frame

    public void FrameStart()
    {
        // if (m_main)
        // {
        //     var desc_heap = Context.DescriptorHeap;
        //     var heaps = stackalloc ID3D12DescriptorHeap*[] { desc_heap.Ptr->ResourceHeap->Heap, desc_heap.Ptr->SamplerHeap->Heap };
        //     list.Handle->SetDescriptorHeaps(2, heaps);
        // }
    }

    public void FrameEnd()
    {
        // m_current_pipeline = null;
    }

    #endregion

    #region Barrier

    public void Barrier(ReadOnlySpan<BufferBarrier> buffer_barrier, ReadOnlySpan<TextureBarrier> texture_barrier = default) =>
        Barrier(texture_barrier, buffer_barrier);
    public void Barrier(ReadOnlySpan<TextureBarrier> texture_barrier, ReadOnlySpan<BufferBarrier> buffer_barrier = default)
    {
        var len = (texture_barrier.Length == 0 ? 0 : 1) + (buffer_barrier.Length == 0 ? 0 : 1);
        if (len == 0) return;
        var groups = stackalloc BarrierGroup[len];
        var index = 0;
        fixed (TextureBarrier* p_tb = texture_barrier)
        fixed (BufferBarrier* p_bb = buffer_barrier)
        {
            if (texture_barrier.Length > 0)
            {
                groups[index++] = new BarrierGroup
                {
                    Type = BarrierType.Texture,
                    NumBarriers = (uint)texture_barrier.Length,
                    PTextureBarriers = p_tb,
                };
            }
            if (buffer_barrier.Length > 0)
            {
                groups[index] = new BarrierGroup
                {
                    Type = BarrierType.Buffer,
                    NumBarriers = (uint)buffer_barrier.Length,
                    PBufferBarriers = p_bb,
                };
            }
            Raw->Barrier((uint)len, groups);
        }
    }

    #endregion

    #region ClearRenderTargetView

    public void ClearRenderTargetView(CpuDescriptorHandle handle, float4 color)
    {
        Raw->ClearRenderTargetView(
            handle,
            ref Unsafe.As<float4, float>(ref color),
            0, null
        );
    }

    #endregion

    #region Pass

    public void BeginRenderPass(
        uint NumRenderTargets,
        RenderPassRenderTargetDesc* pRenderTargets,
        RenderPassDepthStencilDesc* pDepthStencil,
        RenderPassFlags Flags
    ) => Raw->BeginRenderPass(NumRenderTargets, pRenderTargets, pDepthStencil, Flags);

    public void BeginRenderPass(
        ReadOnlySpan<RenderPassRenderTargetDesc> RenderTargets,
        in RenderPassDepthStencilDesc DepthStencil,
        RenderPassFlags Flags
    ) => Raw->BeginRenderPass((uint)RenderTargets.Length, in RenderTargets.GetPinnableReference(), in DepthStencil, Flags);

    public void BeginRenderPass(
        ReadOnlySpan<RenderPassRenderTargetDesc> RenderTargets,
        RenderPassFlags Flags
    ) => Raw->BeginRenderPass((uint)RenderTargets.Length, in RenderTargets.GetPinnableReference(), null, Flags);

    public void EndRenderPass() => Raw->EndRenderPass();

    #endregion

    #region Draw

    public void Draw(
        uint VertexCountPerInstance,
        uint InstanceCount = 1,
        uint StartVertexLocation = 0,
        uint StartInstanceLocation = 0
    ) => list.Handle->DrawInstanced(VertexCountPerInstance, InstanceCount, StartVertexLocation, StartInstanceLocation);

    public void DrawIndexed(
        uint IndexCountPerInstance,
        uint InstanceCount = 1,
        uint StartIndexLocation = 0,
        int BaseVertexLocation = 0,
        uint StartInstanceLocation = 0
    ) => list.Handle->DrawIndexedInstanced(IndexCountPerInstance, InstanceCount, StartIndexLocation, BaseVertexLocation, StartInstanceLocation);

    #endregion

    #region DispatchMesh

    public void DispatchMesh(
        uint ThreadGroupCountX,
        uint ThreadGroupCountY,
        uint ThreadGroupCountZ
    ) => list.Handle->DispatchMesh(ThreadGroupCountX, ThreadGroupCountY, ThreadGroupCountZ);

    #endregion

    #region Dispatch

    public void Dispatch(
        uint ThreadGroupCountX,
        uint ThreadGroupCountY,
        uint ThreadGroupCountZ
    ) => list.Handle->Dispatch(ThreadGroupCountX, ThreadGroupCountY, ThreadGroupCountZ);

    #endregion
}
