using Coplt.Dropping;
using Silk.NET.Input;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;

namespace TestVulkan;

[Dropping]
public unsafe partial class App(IWindow window, GraphicsContext ctx, SwapChain swap_chain)
{
    [Drop]
    public GpuImage? image;

    public void OnLoad()
    {
        var queue_family_index = ctx.QueueFamilyIndex;
        image = new(ctx, new()
        {
            SType = StructureType.ImageCreateInfo,
            Flags = ImageCreateFlags.None,
            ImageType = ImageType.Type2D,
            Format = Format.R8G8B8A8Unorm,
            Extent = new(1024, 1024, 1),
            MipLevels = 1,
            ArrayLayers = 1,
            Samples = SampleCountFlags.Count1Bit,
            Tiling = ImageTiling.Optimal,
            Usage = ImageUsageFlags.SampledBit,
            SharingMode = SharingMode.Exclusive,
            QueueFamilyIndexCount = 1,
            PQueueFamilyIndices = &queue_family_index,
            InitialLayout = ImageLayout.Undefined,
        });
    }

    public void OnUpdate(double delta_time) { }

    public void OnRender(double delta_time)
    {
        ctx.CommandList.Barrier([
            new ImageMemoryBarrier2(
                StructureType.ImageMemoryBarrier2, null,
                PipelineStageFlags2.None, AccessFlags2.None,
                PipelineStageFlags2.ColorAttachmentOutputBit, AccessFlags2.ColorAttachmentWriteBit,
                ImageLayout.Undefined, ImageLayout.ColorAttachmentOptimal,
                ctx.QueueFamilyIndex, ctx.QueueFamilyIndex,
                swap_chain.CurrentImage, new(ImageAspectFlags.ColorBit, 0, 1, 0, 1)
            ),
        ]);

        ctx.CommandList.BeginRenderPass(swap_chain.Size, [
            new()
            {
                SType = StructureType.RenderingAttachmentInfo,
                ImageView = swap_chain.CurrentView,
                ImageLayout = ImageLayout.ColorAttachmentOptimal,
                LoadOp = AttachmentLoadOp.Clear,
                StoreOp = AttachmentStoreOp.Store,
                ClearValue = new(new ClearColorValue(1, 1, 1, 1)),
            },
        ]);
        ctx.CommandList.EndRenderPass();

        ctx.CommandList.Barrier([
            new ImageMemoryBarrier2(
                StructureType.ImageMemoryBarrier2, null,
                PipelineStageFlags2.ColorAttachmentOutputBit, AccessFlags2.ColorAttachmentWriteBit,
                PipelineStageFlags2.None, AccessFlags2.None,
                ImageLayout.ColorAttachmentOptimal, ImageLayout.PresentSrcKhr,
                ctx.QueueFamilyIndex, ctx.QueueFamilyIndex,
                swap_chain.CurrentImage, new(ImageAspectFlags.ColorBit, 0, 1, 0, 1)
            ),
        ]);
    }

    public void OnKeyDown(IKeyboard keyboard, Key key, int key_code) { }
}
