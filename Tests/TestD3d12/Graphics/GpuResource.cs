using Coplt.Dropping;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D12;

namespace TestD3d12;

[Dropping(Unmanaged = true)]
public unsafe partial class GpuResource
{
    #region Fields Props

    private GraphicsContext Graphics { get; }
    [Drop]
    private ComPtr<ID3D12Resource2> m_resource;
    [Drop]
    private ComPtr<D3D12MA.Allocation> m_allocation;

    private ResourceDesc1 m_desc;

    public ref readonly ComPtr<ID3D12Resource2> Resource => ref m_resource;
    public ref readonly ComPtr<D3D12MA.Allocation> Allocation => ref m_allocation;

    public ref readonly ResourceDesc1 Desc => ref m_desc;

    #endregion

    #region Ctor

    public GpuResource(GraphicsContext Graphics, in ResourceDesc1 desc) : this(Graphics, in desc, new()
    {
        Flags = D3D12MA.AllocationFlags.None,
        HeapType = HeapType.Default,
        ExtraHeapFlags = HeapFlags.None,
        CustomPool = null,
        pPrivateData = null
    }) { }

    public GpuResource(GraphicsContext Graphics, in ResourceDesc1 desc, in D3D12MA.AllocationDesc allocation_desc)
    {
        fixed (D3D12MA.AllocationDesc* p_allocation_desc = &allocation_desc)
        fixed (ResourceDesc1* p_desc = &desc)
        {
            Graphics.Allocator.Handle->CreateResource3(
                p_allocation_desc, p_desc,
                BarrierLayout.CopyDest, null, 0, null,
                out m_allocation, out m_resource
            ).TryThrowHResult();
        }
        this.Graphics = Graphics;
        m_desc = desc;
    }

    #endregion
}
