using System.Diagnostics;
using System.Runtime.CompilerServices;
using Coplt.Dropping;
using Coplt.Mathematics;
using Serilog;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Windowing;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace TestVulkan;

[Dropping(Unmanaged = true)]
public unsafe partial class SwapChain
{
    #region Consts

    public const int FrameCount = GraphicsContext.FrameCount;

    #endregion

    #region Field Props

    public GraphicsContext Graphics { get; }
    public IWindow Window { get; }

    private KhrSurface? m_khr_surface;
    private KhrSwapchain? m_khr_swapchain;
    private SurfaceKHR m_surface;
    private SwapchainKHR m_swapchain;

    private int m_semaphore_frame;
    private int m_image_index;
    private int m_next_image_index;

    public KhrSurface? KhrSurface => m_khr_surface;
    public KhrSwapchain? KhrSwapchain => m_khr_swapchain;
    public SurfaceKHR Surface => m_surface;
    public SwapchainKHR Swapchain => m_swapchain;

    public int SemaphoreFrame => m_semaphore_frame;
    public int ImageIndex => m_image_index;

    public Format Format { get; }
    public ColorSpaceKHR ColorSpace { get; }
    public PresentModeKHR PresentMode { get; }

    private uint2 m_cur_size;
    private uint2 m_new_size;

    public uint2 Size => m_cur_size;

    private InlineArray3<Image> m_iamges;
    private InlineArray3<ImageView> m_iamge_views;

    private InlineArray4<Semaphore> m_image_available_semaphores;
    private InlineArray3<Semaphore> m_before_present_semaphores;
    private InlineArray3<ulong> m_fence_values;

    public ReadOnlySpan<Image> Images => m_iamges;
    public ReadOnlySpan<ImageView> ImageViews => m_iamge_views;

    private Lock m_lock = new();

    private SwapchainCreateInfoKHR m_create_info;

    private bool out_date;

    #endregion

    #region Drop

    private void DropImageViews()
    {
        foreach (ref var view in m_iamge_views)
        {
            if (view.Handle == 0) continue;
            Graphics.Vk.DestroyImageView(Graphics.Device, view, null);
            view = default;
        }
    }

    [Drop]
    private void Drop()
    {
        foreach (var semaphore in m_before_present_semaphores)
        {
            if (semaphore.Handle != 0) Graphics.Vk.DestroySemaphore(Graphics.Device, semaphore, null);
        }
        foreach (var semaphore in m_image_available_semaphores)
        {
            if (semaphore.Handle != 0) Graphics.Vk.DestroySemaphore(Graphics.Device, semaphore, null);
        }
        DropImageViews();
        if (m_swapchain.Handle != 0) m_khr_swapchain!.DestroySwapchain(Graphics.Device, m_swapchain, null);
        if (m_surface.Handle != 0) m_khr_surface!.DestroySurface(Graphics.Instance, m_surface, null);
        KhrSwapchain?.Dispose();
        KhrSurface?.Dispose();
    }

    #endregion

    #region Ctor

    public SwapChain(GraphicsContext ctx, IWindow window, uint2 size)
    {
        Graphics = ctx;
        Window = window;
        m_new_size = m_cur_size = size;

        if (size.x == 0 || size.y == 0) throw new ArgumentException("size must be > 0");

        #region Create SwapChain

        {
            if (!ctx.Vk.TryGetInstanceExtension(ctx.Instance, out m_khr_surface))
                throw new NotSupportedException("KHR_surface not supported.");
            if (!ctx.Vk.TryGetDeviceExtension(ctx.Instance, ctx.Device, out m_khr_swapchain))
                throw new NotSupportedException("VK_KHR_swapchain not supported.");

            m_surface = window.VkSurface!.Create<AllocationCallbacks>(ctx.Instance.ToHandle(), null).ToSurface();

            m_khr_surface!.GetPhysicalDeviceSurfaceCapabilities(ctx.PhysicalDevice, m_surface, out var caps);

            uint format_count = 0;
            m_khr_surface.GetPhysicalDeviceSurfaceFormats(ctx.PhysicalDevice, m_surface, &format_count, null);
            if (format_count == 0) throw new UnreachableException("Invalid format count");
            var formats = new SurfaceFormatKHR[format_count];
            fixed (SurfaceFormatKHR* p_formats = formats)
            {
                m_khr_surface.GetPhysicalDeviceSurfaceFormats(ctx.PhysicalDevice, m_surface, &format_count, formats);
            }

            if (ctx.DebugEnabled)
            {
                Log.Information("Support formats: {Formats}", formats.Select(a => (a.Format, a.ColorSpace)));
            }

            SurfaceFormatKHR? selected_format = null;
            foreach (var format in formats)
            {
                if (format.Format is Format.R8G8B8A8Unorm or Format.B8G8R8A8Unorm && format.ColorSpace is ColorSpaceKHR.SpaceSrgbNonlinearKhr)
                {
                    selected_format = format;
                }
            }
            if (selected_format is null) throw new NotSupportedException("No support format.");

            uint present_mode_count = 0;
            m_khr_surface.GetPhysicalDeviceSurfacePresentModes(ctx.PhysicalDevice, m_surface, &present_mode_count, null);
            if (present_mode_count == 0) throw new UnreachableException("Invalid present mode count");
            var present_modes = new PresentModeKHR[present_mode_count];
            fixed (PresentModeKHR* p_present_modes = present_modes)
            {
                m_khr_surface.GetPhysicalDeviceSurfacePresentModes(ctx.PhysicalDevice, m_surface, &present_mode_count, p_present_modes);
            }

            var present_mode = PresentModeKHR.FifoKhr;
            if (present_modes.Any(a => a is PresentModeKHR.MailboxKhr))
            {
                present_mode = PresentModeKHR.MailboxKhr;
            }

            Format = selected_format.Value.Format;
            ColorSpace = selected_format.Value.ColorSpace;
            PresentMode = present_mode;

            SwapchainCreateInfoKHR create_info = new()
            {
                SType = StructureType.SwapchainCreateInfoKhr,
                Surface = m_surface,
                MinImageCount = FrameCount,
                ImageFormat = selected_format.Value.Format,
                ImageColorSpace = selected_format.Value.ColorSpace,
                ImageExtent = new(m_cur_size.x, m_cur_size.y),
                ImageArrayLayers = 1,
                ImageUsage = ImageUsageFlags.ColorAttachmentBit,
                ImageSharingMode = SharingMode.Exclusive,
                QueueFamilyIndexCount = 0,
                PQueueFamilyIndices = null,
                PreTransform = caps.CurrentTransform,
                CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
                PresentMode = present_mode,
            };
            m_create_info = create_info;

            m_khr_swapchain!.CreateSwapchain(ctx.Device, &create_info, null, out m_swapchain).TryThrow();
        }

        #endregion

        #region Create Semaphore Fence

        {
            SemaphoreCreateInfo info = new()
            {
                SType = StructureType.SemaphoreCreateInfo,
            };
            for (var i = 0; i < FrameCount; i++)
            {
                ctx.Vk.CreateSemaphore(ctx.Device, &info, null, out m_before_present_semaphores[i]).TryThrow();
            }
            for (var i = 0; i < 4; i++)
            {
                ctx.Vk.CreateSemaphore(ctx.Device, &info, null, out m_image_available_semaphores[i]).TryThrow();
            }
        }

        #endregion

        CreateImageViews();

        #region AcquireNextImage

        {
            var image_available_semaphore = m_image_available_semaphores[3];
            uint image_index;
            m_khr_swapchain!.AcquireNextImage(Graphics.Device, m_swapchain, ulong.MaxValue, image_available_semaphore, default, &image_index).TryThrow();

            {
                var wait_stages = PipelineStageFlags.ColorAttachmentOutputBit;
                SubmitInfo info = new()
                {
                    SType = StructureType.SubmitInfo,
                    WaitSemaphoreCount = 1,
                    PWaitSemaphores = &image_available_semaphore,
                    PWaitDstStageMask = &wait_stages,
                    CommandBufferCount = 0,
                    PCommandBuffers = null,
                    SignalSemaphoreCount = 0,
                    PSignalSemaphores = null,
                };
                Graphics.Vk.QueueSubmit(Graphics.Queue, 1, &info, default);
            }
        }

        #endregion
    }

    #endregion

    #region Create Images

    private void CreateImageViews()
    {
        fixed (Image* p_images = Images)
        {
            uint count = FrameCount;
            m_khr_swapchain!.GetSwapchainImages(Graphics.Device, m_swapchain, &count, p_images).TryThrow();
        }
        for (var i = 0; i < FrameCount; i++)
        {
            ImageViewCreateInfo create_info = new()
            {
                SType = StructureType.ImageViewCreateInfo,
                Image = m_iamges[i],
                ViewType = ImageViewType.Type2D,
                Format = Format,
                Components = new(ComponentSwizzle.Identity, ComponentSwizzle.Identity, ComponentSwizzle.Identity, ComponentSwizzle.Identity),
                SubresourceRange =
                {
                    AspectMask = ImageAspectFlags.ColorBit,
                    BaseMipLevel = 0,
                    LevelCount = 1,
                    BaseArrayLayer = 0,
                    LayerCount = 1,
                },
            };

            Graphics.Vk.CreateImageView(Graphics.Device, &create_info, null, out m_iamge_views[i]).TryThrow();
        }
    }

    #endregion

    #region Render

    public Image CurrentImage => m_iamges[m_image_index];
    public ImageView CurrentView => m_iamge_views[m_image_index];

    #endregion

    #region Ctrl

    public void OnResize(uint2 size)
    {
        if (size.x == 0 || size.y == 0) throw new ArgumentException("size must be > 0");
        Interlocked.Exchange(
            ref Unsafe.As<uint2, ulong>(ref m_new_size),
            Unsafe.BitCast<uint2, ulong>(size)
        );
    }

    public void Present()
    {
        using var _ = m_lock.EnterScope();
        PresentNoWait_InLock();
        WaitFrameReady_InLock();
    }

    public void PresentNoWait()
    {
        using var _ = m_lock.EnterScope();
        PresentNoWait_InLock();
    }

    public void WaitFrameReady()
    {
        using var _ = m_lock.EnterScope();
        WaitFrameReady_InLock();
    }

    private void PresentNoWait_InLock()
    {
        var before_present_semaphore = m_before_present_semaphores[m_image_index];

        {
            SubmitInfo info = new()
            {
                SType = StructureType.SubmitInfo,
                WaitSemaphoreCount = 0,
                PWaitSemaphores = null,
                PWaitDstStageMask = null,
                CommandBufferCount = 0,
                PCommandBuffers = null,
                SignalSemaphoreCount = 1,
                PSignalSemaphores = &before_present_semaphore,
            };
            Graphics.Vk.QueueSubmit(Graphics.Queue, 1, &info, default).TryThrow();
        }

        {
            var swapchain = m_swapchain;
            var image_index = (uint)m_image_index;
            PresentInfoKHR present_info = new()
            {
                SType = StructureType.PresentInfoKhr,
                PNext = null,
                WaitSemaphoreCount = 1,
                PWaitSemaphores = &before_present_semaphore,
                SwapchainCount = 1,
                PSwapchains = &swapchain,
                PImageIndices = &image_index,
            };
            m_khr_swapchain!.QueuePresent(Graphics.Queue, &present_info).TryThrow();
        }

        {
            var semaphore = m_image_available_semaphores[m_semaphore_frame++ % 4];
            uint image_index;
            var result = m_khr_swapchain!.AcquireNextImage(Graphics.Device, m_swapchain, ulong.MaxValue, semaphore, default, &image_index);
            if (result == Result.ErrorOutOfDateKhr) out_date = true;
            else result.TryThrow();
            m_next_image_index = (int)image_index;

            var fence_value = Graphics.AllocSignal();
            {
                var fence = Graphics.Fence;
                var wait_stages = PipelineStageFlags.None;
                SubmitInfo info = new()
                {
                    SType = StructureType.SubmitInfo,
                    WaitSemaphoreCount = 1,
                    PWaitSemaphores = &semaphore,
                    PWaitDstStageMask = &wait_stages,
                    CommandBufferCount = 0,
                    PCommandBuffers = null,
                    SignalSemaphoreCount = 1,
                    PSignalSemaphores = &fence,
                };
                info.AddNext(out TimelineSemaphoreSubmitInfo ex_info);
                ex_info.SignalSemaphoreValueCount = 1;
                ex_info.PSignalSemaphoreValues = &fence_value;
                Graphics.Vk.QueueSubmit(Graphics.Queue, 1, &info, default).TryThrow();
            }

            m_fence_values[m_next_image_index] = fence_value;
        }
    }

    private void WaitFrameReady_InLock()
    {
        {
            var cur_size = m_cur_size;
            var new_size = Unsafe.BitCast<ulong, uint2>(Interlocked.Read(ref Unsafe.As<uint2, ulong>(ref m_new_size)));
            if (!cur_size.Equals(new_size) && out_date)
            {
                DoResize_InLock(new_size);
            }
        }
        var min = ulong.MaxValue;
        for (var i = 0; i < FrameCount; i++)
        {
            var v = m_fence_values[i];
            if (v < min) min = v;
        }
        Graphics.WaitOnCpu(min);
        m_image_index = m_next_image_index;
    }

    private void DoResize_InLock(uint2 size)
    {
        if (size.x == 0 || size.y == 0) throw new ArgumentException("size must be > 0");

        WaitAll_InLock();
        DropImageViews();

        if (m_swapchain.Handle != 0) m_khr_swapchain!.DestroySwapchain(Graphics.Device, m_swapchain, null);

        m_create_info.ImageExtent = new(size.x, size.y);
        var create_info = m_create_info;

        m_khr_swapchain!.CreateSwapchain(Graphics.Device, &create_info, null, out m_swapchain).TryThrow();
        CreateImageViews();

        m_cur_size = size;
        
        #region AcquireNextImage

        {
            var image_available_semaphore = m_image_available_semaphores[3];
            uint image_index;
            m_khr_swapchain!.AcquireNextImage(Graphics.Device, m_swapchain, ulong.MaxValue, image_available_semaphore, default, &image_index).TryThrow();

            {
                var wait_stages = PipelineStageFlags.ColorAttachmentOutputBit;
                SubmitInfo info = new()
                {
                    SType = StructureType.SubmitInfo,
                    WaitSemaphoreCount = 1,
                    PWaitSemaphores = &image_available_semaphore,
                    PWaitDstStageMask = &wait_stages,
                    CommandBufferCount = 0,
                    PCommandBuffers = null,
                    SignalSemaphoreCount = 0,
                    PSignalSemaphores = null,
                };
                Graphics.Vk.QueueSubmit(Graphics.Queue, 1, &info, default);
            }
        }

        #endregion
    }

    private void WaitAll_InLock()
    {
        ulong max = 0;
        for (var i = 0; i < FrameCount; i++)
        {
            var v = m_fence_values[i];
            if (v > max) max = v;
        }
        Graphics.WaitOnCpu(max);
    }

    [Drop(Order = -1)]
    public void WaitAll()
    {
        using var _ = m_lock.EnterScope();
        WaitAll_InLock();
    }

    #endregion
}
