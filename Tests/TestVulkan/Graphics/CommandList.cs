using Coplt.Dropping;
using Coplt.Mathematics;
using Silk.NET.Vulkan;

namespace TestVulkan;

[Dropping(Unmanaged = true)]
public unsafe partial class CommandList
{
    #region Fields

    public GraphicsContext Graphics { get; }

    public CommandBuffer CommandBuffer => Graphics.CommandBuffers[Graphics.CurrentFrame];

    #endregion

    #region Ctor

    public CommandList(GraphicsContext graphics)
    {
        Graphics = graphics;
    }

    #endregion

    #region Frame

    public void FrameStart()
    {
        CommandBufferBeginInfo info = new()
        {
            SType = StructureType.CommandBufferBeginInfo,
            Flags = CommandBufferUsageFlags.OneTimeSubmitBit,
            PInheritanceInfo = null,
        };
        Graphics.Vk.BeginCommandBuffer(CommandBuffer, &info);
    }

    public void FrameEnd()
    {
        Graphics.Vk.EndCommandBuffer(CommandBuffer);
    }

    #endregion

    #region Barrier

    public void Barrier(ReadOnlySpan<BufferMemoryBarrier2> buffer_barrier, ReadOnlySpan<ImageMemoryBarrier2> texture_barrier = default) =>
        Barrier(texture_barrier, buffer_barrier);
    public void Barrier(ReadOnlySpan<ImageMemoryBarrier2> texture_barrier, ReadOnlySpan<BufferMemoryBarrier2> buffer_barrier = default)
    {
        fixed (ImageMemoryBarrier2* p_texture_barrier = texture_barrier)
        fixed (BufferMemoryBarrier2* p_buffer_barrier = buffer_barrier)
        {
            DependencyInfo info = new()
            {
                SType = StructureType.DependencyInfo,
                DependencyFlags = DependencyFlags.None,
                MemoryBarrierCount = 0,
                PMemoryBarriers = null,
                BufferMemoryBarrierCount = (uint)buffer_barrier.Length,
                PBufferMemoryBarriers = p_buffer_barrier,
                ImageMemoryBarrierCount = (uint)texture_barrier.Length,
                PImageMemoryBarriers = p_texture_barrier,
            };
            Graphics.Vk.CmdPipelineBarrier2(CommandBuffer, &info);
        }
    }

    #endregion

    #region ClearImage

    public void ClearColorImage(Image image, ImageLayout layout, float4 color, params ReadOnlySpan<ImageSubresourceRange> ranges)
    {
        ClearColorValue value = new()
        {
            Float32_0 = color.x,
            Float32_1 = color.y,
            Float32_2 = color.z,
            Float32_3 = color.w,
        };
        fixed (ImageSubresourceRange* p_ranges = ranges)
        {
            Graphics.Vk.CmdClearColorImage(CommandBuffer, image, layout, &value, (uint)ranges.Length, p_ranges);
        }
    }

    #endregion

    #region Pass

    public void BeginRenderPass(
        Rect2D RenderArea,
        uint ColorAttachmentCount,
        RenderingAttachmentInfo* PColorAttachments,
        RenderingAttachmentInfo* PDepthAttachment = null,
        RenderingAttachmentInfo* PStencilAttachment = null,
        RenderingFlags Flags = RenderingFlags.None,
        uint LayerCount = 1,
        uint ViewMask = 0
    )
    {
        RenderingInfo info = new()
        {
            SType = StructureType.RenderingInfo,
            Flags = Flags,
            RenderArea = RenderArea,
            LayerCount = LayerCount,
            ViewMask = ViewMask,
            ColorAttachmentCount = ColorAttachmentCount,
            PColorAttachments = PColorAttachments,
            PDepthAttachment = PDepthAttachment,
            PStencilAttachment = PStencilAttachment,
        };
        Graphics.Vk.CmdBeginRendering(CommandBuffer, &info);
    }

    public void BeginRenderPass(
        uint2 RenderAreaSize,
        ReadOnlySpan<RenderingAttachmentInfo> ColorAttachments,
        RenderingFlags Flags = RenderingFlags.None,
        uint LayerCount = 1,
        uint ViewMask = 0
    ) => BeginRenderPass(new Rect2D(default, new(RenderAreaSize.x, RenderAreaSize.y)), ColorAttachments, Flags, LayerCount, ViewMask);

    public void BeginRenderPass(
        Rect2D RenderArea,
        ReadOnlySpan<RenderingAttachmentInfo> ColorAttachments,
        RenderingFlags Flags = RenderingFlags.None,
        uint LayerCount = 1,
        uint ViewMask = 0
    )
    {
        fixed (RenderingAttachmentInfo* p_color_attachments = ColorAttachments)
        {
            RenderingInfo info = new()
            {
                SType = StructureType.RenderingInfo,
                Flags = Flags,
                RenderArea = RenderArea,
                LayerCount = LayerCount,
                ViewMask = ViewMask,
                ColorAttachmentCount = (uint)ColorAttachments.Length,
                PColorAttachments = p_color_attachments,
                PDepthAttachment = null,
                PStencilAttachment = null,
            };
            Graphics.Vk.CmdBeginRendering(CommandBuffer, &info);
        }
    }

    public void EndRenderPass() => Graphics.Vk.CmdEndRendering(CommandBuffer);

    #endregion

    #region Draw

    public void Draw(
        uint VertexCountPerInstance,
        uint InstanceCount = 1,
        uint StartVertexLocation = 0,
        uint StartInstanceLocation = 0
    ) => Graphics.Vk.CmdDraw(CommandBuffer, VertexCountPerInstance, InstanceCount, StartVertexLocation, StartInstanceLocation);

    public void DrawIndexed(
        uint IndexCountPerInstance,
        uint InstanceCount = 1,
        uint StartIndexLocation = 0,
        int BaseVertexLocation = 0,
        uint StartInstanceLocation = 0
    ) => Graphics.Vk.CmdDrawIndexed(CommandBuffer, IndexCountPerInstance, InstanceCount, StartIndexLocation, BaseVertexLocation, StartInstanceLocation);

    #endregion

    #region Dispatch

    public void Dispatch(
        uint ThreadGroupCountX,
        uint ThreadGroupCountY,
        uint ThreadGroupCountZ
    ) => Graphics.Vk.CmdDispatch(CommandBuffer, ThreadGroupCountX, ThreadGroupCountY, ThreadGroupCountZ);

    #endregion
}
