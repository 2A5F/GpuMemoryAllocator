using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Coplt.Dropping;
using Serilog;
using Serilog.Events;
using Silk.NET.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace TestVulkan;

[Dropping(Unmanaged = true)]
public unsafe partial class GraphicsContext
{
    #region Consts

    public const int FrameCount = 3;

    #endregion

    #region Props Fields

    public Vk Vk { get; }

    public bool DebugEnabled { get; }

    private ExtDebugUtils? m_debug_utils;

    private Instance m_instance;
    private DebugUtilsMessengerEXT m_debug_messenger;

    private PhysicalDevice m_physical_device;
    private Device m_device;

    private Queue m_queue;

    private Semaphore m_fence;
    private InlineArray3<ulong> m_fence_values = default;
    private ulong fence_value;
    private int m_cur_frame;

    private uint m_queue_family_index;

    private InlineArray3<CommandPool> m_command_pools;
    private InlineArray3<CommandBuffer> m_command_buffers;

    private CommandList m_command_list;
    
    private Vma.Allocator* m_allocator;

    private readonly Queue<IGpuRecyclable> m_recycle_queue = new();
    private readonly Lock m_recycle_lock = new();

    public ExtDebugUtils? DebugUtils => m_debug_utils;
    public ref readonly Instance Instance => ref m_instance;
    public ref readonly DebugUtilsMessengerEXT DebugUtilsMessenger => ref m_debug_messenger;

    public ref readonly PhysicalDevice PhysicalDevice => ref m_physical_device;
    public ref readonly Device Device => ref m_device;
    public ref readonly Queue Queue => ref m_queue;

    public ref readonly Semaphore Fence => ref m_fence;
    public ReadOnlySpan<ulong> FenceValues => m_fence_values;
    public int CurrentFrame => m_cur_frame;

    public uint QueueFamilyIndex => m_queue_family_index;

    public ReadOnlySpan<CommandPool> CommandPools => m_command_pools;
    public ReadOnlySpan<CommandBuffer> CommandBuffers => m_command_buffers;

    public CommandList CommandList => m_command_list;

    #endregion

    #region Drop

    [Drop]
    private void Drop()
    {
        if (m_allocator != null) Vma.Apis.DestroyAllocator(m_allocator);
        for (var i = 0; i < FrameCount; i++)
        {
            if (m_command_buffers[i].Handle != 0)
                Vk.FreeCommandBuffers(Device, m_command_pools[i], [m_command_buffers[i]]);
        }
        foreach (var pool in m_command_pools)
        {
            if (pool.Handle != 0) Vk.DestroyCommandPool(Device, pool, null);
        }
        if (m_fence.Handle != 0) Vk.DestroySemaphore(Device, m_fence, null);
        if (m_device.Handle != 0) Vk.DestroyDevice(m_device, null);
        if (m_debug_messenger.Handle != 0) m_debug_utils!.DestroyDebugUtilsMessenger(m_instance, m_debug_messenger, null);
        if (m_instance.Handle != 0) Vk.DestroyInstance(m_instance, null);
    }

    #endregion

    #region Ctor

    public GraphicsContext(Vk vk, IVkSurface surface, bool debug)
    {
        Vk = vk;

        #region Create instance

        {
            #region Query supported layers

            uint layout_count = 0;
            vk.EnumerateInstanceLayerProperties(&layout_count, null);
            var a_supported_layers = new LayerProperties[layout_count];
            fixed (LayerProperties* p_supported_layers = a_supported_layers)
            {
                vk.EnumerateInstanceLayerProperties(&layout_count, p_supported_layers);
            }
            var supported_layers = a_supported_layers.Select(a => new string((sbyte*)a.LayerName)).ToHashSet();

            #endregion

            var pp_surface_exts = surface.GetRequiredExtensions(out var num_surface_exts);

            var exts = new List<IntPtr>((int)num_surface_exts);
            var layouts = new List<IntPtr>();

            // ReSharper disable once RedundantExplicitParamsArrayCreation
            exts.AddRange(new ReadOnlySpan<IntPtr>((IntPtr*)pp_surface_exts, (int)num_surface_exts));

            if (debug)
            {
                if (supported_layers.Contains("VK_LAYER_KHRONOS_validation"))
                {
                    DebugEnabled = true;
                    exts.Add((IntPtr)Utils.Lit("VK_EXT_debug_utils"u8));
                    layouts.Add((IntPtr)Utils.Lit("VK_LAYER_KHRONOS_validation"u8));
                }
            }

            fixed (IntPtr* p_exts = CollectionsMarshal.AsSpan(exts))
            fixed (IntPtr* p_layouts = CollectionsMarshal.AsSpan(layouts))
            {
                ApplicationInfo app_info = new()
                {
                    SType = StructureType.ApplicationInfo,
                    PApplicationName = Utils.Lit("Test Vulkan"u8),
                    ApplicationVersion = new Version32(0, 0, 0),
                    PEngineName = Utils.Lit("No Engine"u8),
                    EngineVersion = new Version32(0, 0, 0),
                    ApiVersion = Vk.Version13,
                };

                InstanceCreateInfo create_info = new()
                {
                    SType = StructureType.InstanceCreateInfo,
                    PApplicationInfo = &app_info,
                    EnabledExtensionCount = (uint)exts.Count,
                    PpEnabledExtensionNames = (byte**)p_exts,
                    EnabledLayerCount = (uint)layouts.Count,
                    PpEnabledLayerNames = (byte**)p_layouts,
                };
                var create_chain = Utils.ChainStart(&create_info);

                DebugUtilsMessengerCreateInfoEXT debug_create_info = default;
                if (DebugEnabled)
                {
                    Utils.Chain(ref create_chain, &debug_create_info);
                    debug_create_info.MessageSeverity =
                        DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt |
                        DebugUtilsMessageSeverityFlagsEXT.WarningBitExt;
                    debug_create_info.MessageType =
                        DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                        DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
                        DebugUtilsMessageTypeFlagsEXT.ValidationBitExt;
                    debug_create_info.PfnUserCallback = new(&DebugCallback);
                }

                vk.CreateInstance(&create_info, null, out m_instance).TryThrow();
            }
        }

        #endregion

        #region Create Debug Messenger

        if (DebugEnabled && vk.TryGetInstanceExtension(m_instance, out m_debug_utils))
        {
            DebugUtilsMessengerCreateInfoEXT debug_create_info = new()
            {
                SType = StructureType.DebugUtilsMessengerCreateInfoExt,
                MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.WarningBitExt,
                MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                              DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
                              DebugUtilsMessageTypeFlagsEXT.ValidationBitExt,
                PfnUserCallback = new(&DebugCallback),
            };

            m_debug_utils!.CreateDebugUtilsMessenger(m_instance, &debug_create_info, null, out m_debug_messenger).TryThrow();
        }

        #endregion

        #region Select Physical Device

        {
            var physical_devices = vk.GetPhysicalDevices(m_instance).OrderByDescending(pd =>
            {
                var props = vk.GetPhysicalDeviceProperties(pd);
                if (DebugEnabled)
                {
                    var name = new string((sbyte*)props.DeviceName);
                    Log.Information("Exists Physical Device \"{Name}\" {{ VendorID = {VendorID}, DeviceID = {DeviceID}, Type = {Type}, DeviceVersion = {DeviceVersion}, ApiVersion = {ApiVersion}, Id = {PipelineCacheId} }}",
                        name, props.VendorID, props.DeviceID, props.DeviceType, Utils.DecodeVersion(props.DriverVersion), Utils.DecodeVersion(props.ApiVersion), *(Guid*)props.PipelineCacheUuid);
                }
                var source = props.DeviceType switch
                {
                    PhysicalDeviceType.DiscreteGpu => 1000,
                    PhysicalDeviceType.IntegratedGpu => 0,
                    _ => -1000,
                };
                var ver = Utils.SplitVersion(props.ApiVersion);
                return (source, ver);
            }).ToArray();

            if (physical_devices.Length == 0) throw new NotSupportedException("No physical devices found");
            m_physical_device = physical_devices[0];
            if (DebugEnabled)
            {
                var props = vk.GetPhysicalDeviceProperties(m_physical_device);
                var name = new string((sbyte*)props.DeviceName);
                Log.Information("Selected Physical Device \"{Name}\" {{ VendorID = {VendorID}, DeviceID = {DeviceID}, Type = {Type}, DeviceVersion = {DeviceVersion}, ApiVersion = {ApiVersion}, Id = {PipelineCacheId} }}",
                    name, props.VendorID, props.DeviceID, props.DeviceType, Utils.DecodeVersion(props.DriverVersion), Utils.DecodeVersion(props.ApiVersion), *(Guid*)props.PipelineCacheUuid);
            }
        }

        #endregion

        #region Create Device

        {
            uint queue_family_count = 0;
            vk.GetPhysicalDeviceQueueFamilyProperties(m_physical_device, &queue_family_count, null);
            if (queue_family_count == 0) throw new UnreachableException("no queue families found");
            var queue_families = new QueueFamilyProperties[queue_family_count];
            fixed (QueueFamilyProperties* p_queue_families = queue_families)
            {
                vk.GetPhysicalDeviceQueueFamilyProperties(m_physical_device, &queue_family_count, p_queue_families);
            }
            uint queue_family_index = 0;
            for (; queue_family_index < queue_family_count; queue_family_index++)
            {
                if (queue_families[queue_family_index].QueueFlags.HasFlag(QueueFlags.GraphicsBit)) break;
            }
            m_queue_family_index = queue_family_index;

            var exts = new List<IntPtr>();
            var layers = new List<IntPtr>();

            exts.Add((IntPtr)Utils.Lit("VK_KHR_swapchain"u8));

            if (DebugEnabled)
            {
                layers.Add((IntPtr)Utils.Lit("VK_LAYER_KHRONOS_validation"u8));
            }

            fixed (IntPtr* p_exts = CollectionsMarshal.AsSpan(exts))
            fixed (IntPtr* p_layers = CollectionsMarshal.AsSpan(layers))
            {
                PhysicalDeviceFeatures features = new();
                float queue_priority = 1;
                DeviceQueueCreateInfo queue_create_info = new()
                {
                    SType = StructureType.DeviceQueueCreateInfo,
                    PQueuePriorities = &queue_priority,
                    QueueFamilyIndex = queue_family_index,
                    QueueCount = 1,
                };
                DeviceCreateInfo device_create_info = new()
                {
                    SType = StructureType.DeviceCreateInfo,
                    QueueCreateInfoCount = 1,
                    PQueueCreateInfos = &queue_create_info,
                    PEnabledFeatures = &features,
                    EnabledExtensionCount = (uint)exts.Count,
                    PpEnabledExtensionNames = (byte**)p_exts,
                    EnabledLayerCount = (uint)layers.Count,
                    PpEnabledLayerNames = (byte**)p_layers,
                };
                device_create_info.AddNext(out PhysicalDeviceTimelineSemaphoreFeatures timeline);
                timeline.TimelineSemaphore = true;
                device_create_info.AddNext(out PhysicalDeviceSynchronization2Features sync2);
                sync2.Synchronization2 = true;
                device_create_info.AddNext(out PhysicalDeviceDynamicRenderingFeatures dyn_render);
                dyn_render.DynamicRendering = true;
                device_create_info.AddNext(out PhysicalDeviceBufferDeviceAddressFeatures bda);
                bda.BufferDeviceAddress = true;

                vk.CreateDevice(m_physical_device, &device_create_info, null, out m_device).TryThrow();

                vk.GetDeviceQueue(m_device, queue_family_index, 0, out m_queue);
            }
        }

        #endregion

        #region Create Fence

        {
            SemaphoreCreateInfo info = new()
            {
                SType = StructureType.SemaphoreCreateInfo,
            };
            info.AddNext(out SemaphoreTypeCreateInfo type_info);
            type_info.InitialValue = 0;
            type_info.SemaphoreType = SemaphoreType.Timeline;
            Vk.CreateSemaphore(Device, &info, null, out m_fence).TryThrow();
        }

        #endregion

        #region Create Cmd

        {
            CommandPoolCreateInfo info = new()
            {
                SType = StructureType.CommandPoolCreateInfo,
                Flags = CommandPoolCreateFlags.None,
                QueueFamilyIndex = m_queue_family_index,
            };
            for (var i = 0; i < FrameCount; i++)
            {
                Vk.CreateCommandPool(m_device, &info, null, out m_command_pools[i]);
            }
        }
        {
            for (var i = 0; i < FrameCount; i++)
            {
                CommandBufferAllocateInfo info = new()
                {
                    SType = StructureType.CommandBufferAllocateInfo,
                    CommandPool = m_command_pools[i],
                    Level = CommandBufferLevel.Primary,
                    CommandBufferCount = 1,
                };
                Vk.AllocateCommandBuffers(m_device, &info, out m_command_buffers[i]);
            }
        }

        m_command_list = new(this);

        #endregion

        #region Create Allactor

        {
            Vma.AllocatorCreateInfo info = new()
            {
                Flags = Vma.AllocatorCreateFlags.BufferDeviceAddressBit,
                PhysicalDevice = m_physical_device,
                Device = m_device,
                Instance = m_instance,
                VulkanApiVersion = Vk.Version13,
            };
            Vma.Allocator* allocator;
            Vma.Apis.CreateAllocator(&info, &allocator).TryThrow();
            m_allocator = allocator;
        }

        #endregion
    }

    #endregion

    #region DebugCallback

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static Bool32 DebugCallback(
        DebugUtilsMessageSeverityFlagsEXT messageSeverity,
        DebugUtilsMessageTypeFlagsEXT messageTypes,
        DebugUtilsMessengerCallbackDataEXT* pCallbackData,
        void* pUserData
    )
    {
        var level = messageSeverity switch
        {
            DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt => LogEventLevel.Debug,
            DebugUtilsMessageSeverityFlagsEXT.InfoBitExt => LogEventLevel.Information,
            DebugUtilsMessageSeverityFlagsEXT.WarningBitExt => LogEventLevel.Warning,
            DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt => LogEventLevel.Error,
            _ => LogEventLevel.Information,
        };
        if (Log.IsEnabled(level))
        {
            var msg = new string((sbyte*)pCallbackData->PMessage);
            Log.Write(level, "{Msg}", msg);
        }
        return Vk.False;
    }

    #endregion

    #region Fence

    public ulong AllocSignal() => Interlocked.Increment(ref fence_value);

    public ulong SignalOnGpu()
    {
        var value = AllocSignal();
        var semaphore = m_fence;
        SubmitInfo info = new()
        {
            SType = StructureType.SubmitInfo,
            WaitSemaphoreCount = 0,
            PWaitSemaphores = null,
            PWaitDstStageMask = null,
            CommandBufferCount = 0,
            PCommandBuffers = null,
            SignalSemaphoreCount = 1,
            PSignalSemaphores = &semaphore,
        };
        info.AddNext(out TimelineSemaphoreSubmitInfo ex_info);
        ex_info.SignalSemaphoreValueCount = 1;
        ex_info.PSignalSemaphoreValues = &value;
        Vk.QueueSubmit(Queue, 1, &info, default).TryThrow();
        return value;
    }

    public ulong SignalOnCpu()
    {
        var value = AllocSignal();
        Utils.TimelineSingle(this, m_fence, value);
        return value;
    }

    public void WaitOnGpu(ulong value)
    {
        var semaphore = m_fence;
        var wait_stages = PipelineStageFlags.AllCommandsBit;
        SubmitInfo info = new()
        {
            SType = StructureType.SubmitInfo,
            WaitSemaphoreCount = 1,
            PWaitSemaphores = &semaphore,
            PWaitDstStageMask = &wait_stages,
            CommandBufferCount = 0,
            PCommandBuffers = null,
            SignalSemaphoreCount = 0,
            PSignalSemaphores = null,
        };
        info.AddNext(out TimelineSemaphoreSubmitInfo ex_info);
        ex_info.SignalSemaphoreValueCount = 1;
        ex_info.PSignalSemaphoreValues = &value;
        Vk.QueueSubmit(Queue, 1, &info, default).TryThrow();
    }

    public void WaitOnCpu(ulong value)
    {
        Utils.TimelineWait(this, m_fence, value);
    }

    [Drop(Order = -1000)]
    private void WaitAll()
    {
        WaitOnCpu(SignalOnGpu());
    }

    #endregion

    #region Submit

    public void Submit()
    {
        var value = AllocSignal();
        m_fence_values[m_cur_frame] = value;
        var fence = m_fence;
        var cmd = m_command_buffers[m_cur_frame];
        SubmitInfo info = new()
        {
            SType = StructureType.SubmitInfo,
            WaitSemaphoreCount = 0,
            PWaitSemaphores = null,
            PWaitDstStageMask = null,
            CommandBufferCount = 1,
            PCommandBuffers = &cmd,
            SignalSemaphoreCount = 1,
            PSignalSemaphores = &fence,
        };
        info.AddNext(out TimelineSemaphoreSubmitInfo ex_info);
        ex_info.SignalSemaphoreValueCount = 1;
        ex_info.PSignalSemaphoreValues = &value;
        Vk.QueueSubmit(m_queue, 1, &info, default);
    }

    public void SubmitNotEnd()
    {
        var cmd = m_command_buffers[m_cur_frame];
        SubmitInfo info = new()
        {
            SType = StructureType.SubmitInfo,
            WaitSemaphoreCount = 0,
            PWaitSemaphores = null,
            PWaitDstStageMask = null,
            CommandBufferCount = 1,
            PCommandBuffers = &cmd,
            SignalSemaphoreCount = 0,
            PSignalSemaphores = null,
        };
        Vk.QueueSubmit(m_queue, 1, &info, default);
    }

    public void EndFrame()
    {
        m_fence_values[m_cur_frame] = SignalOnGpu();
    }

    public void ReadyNextFrame()
    {
        m_cur_frame++;
        if (m_cur_frame >= FrameCount) m_cur_frame = 0;
        var value = m_fence_values[m_cur_frame];
        WaitOnCpu(value);
        Vk.ResetCommandPool(m_device, m_command_pools[m_cur_frame], CommandPoolResetFlags.None);
        Recycle();
    }

    public void ReadyNextFrameNoWait()
    {
        m_cur_frame++;
        if (m_cur_frame >= FrameCount) m_cur_frame = 0;
        Vk.ResetCommandPool(m_device, m_command_pools[m_cur_frame], CommandPoolResetFlags.None);
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
