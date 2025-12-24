using Coplt.Dropping;
using Silk.NET.Vulkan;

namespace TestVulkan;

[Dropping(Unmanaged = true)]
public unsafe partial class GpuImage
{
    #region Fields Props

    private GraphicsContext Graphics { get; }
    private Image m_image;
    private Vma.Allocation* m_allocation;
    private Vma.AllocationInfo m_allocation_info;

    public Image Image => m_image;
    public Vma.Allocation* Allocation => m_allocation;
    public ref readonly Vma.AllocationInfo AllocationInfo => ref m_allocation_info;

    #endregion

    #region Drop

    [Drop]
    private void Drop()
    {
        if (m_allocation != null)
        {
            Vma.Apis.DestroyImage(Graphics.Allocator, m_image, m_allocation);
        }
    }

    #endregion

    #region Ctor

    public GpuImage(GraphicsContext Graphics, in ImageCreateInfo info) : this(Graphics, in info, new() { Usage = Vma.MemoryUsage.Auto }) { }

    public GpuImage(GraphicsContext Graphics, in ImageCreateInfo info, in Vma.AllocationCreateInfo allocation_create_info)
    {
        fixed (Vma.AllocationCreateInfo* p_allocation_info = &allocation_create_info)
        fixed (ImageCreateInfo* p_info = &info)
        {
            Image image;
            Vma.Allocation* allocation;
            Vma.AllocationInfo allocation_info;
            Vma.Apis.CreateImage(Graphics.Allocator, p_info, p_allocation_info, &image, &allocation, &allocation_info).TryThrow();
            m_image = image;
            m_allocation = allocation;
            m_allocation_info = allocation_info;
        }
        this.Graphics = Graphics;
    }

    #endregion
}
