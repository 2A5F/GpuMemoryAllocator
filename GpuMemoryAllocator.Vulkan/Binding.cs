namespace Vma;
[Flags]
public enum AllocatorCreateFlags : int
{
    ExternallySynchronizedBit = 0x00000001,
    KhrDedicatedAllocationBit = 0x00000002,
    KhrBindMemory2Bit = 0x00000004,
    ExtMemoryBudgetBit = 0x00000008,
    AmdDeviceCoherentMemoryBit = 0x00000010,
    BufferDeviceAddressBit = 0x00000020,
    ExtMemoryPriorityBit = 0x00000040,
    KhrMaintenance4Bit = 0x00000080,
    KhrMaintenance5Bit = 0x00000100,
    KhrExternalMemoryWin32Bit = 0x00000200,
}
public enum MemoryUsage : int
{
    Unknown = 0,
    GpuOnly = 1,
    CpuOnly = 2,
    CpuToGpu = 3,
    GpuToCpu = 4,
    CpuCopy = 5,
    GpuLazilyAllocated = 6,
    Auto = 7,
    AutoPreferDevice = 8,
    AutoPreferHost = 9,
}
[Flags]
public enum AllocationCreateFlags : int
{
    DedicatedMemoryBit = 0x00000001,
    NeverAllocateBit = 0x00000002,
    MappedBit = 0x00000004,
    UserDataCopyStringBit = 0x00000020,
    UpperAddressBit = 0x00000040,
    DontBindBit = 0x00000080,
    WithinBudgetBit = 0x00000100,
    CanAliasBit = 0x00000200,
    HostAccessSequentialWriteBit = 0x00000400,
    HostAccessRandomBit = 0x00000800,
    HostAccessAllowTransferInsteadBit = 0x00001000,
    StrategyMinMemoryBit = 0x00010000,
    StrategyMinTimeBit = 0x00020000,
    StrategyMinOffsetBit = 0x00040000,
    StrategyBestFitBit = StrategyMinMemoryBit,
    StrategyFirstFitBit = StrategyMinTimeBit,
    StrategyMask = StrategyMinMemoryBit | StrategyMinTimeBit | StrategyMinOffsetBit,
}
[Flags]
public enum PoolCreateFlags : int
{
    IgnoreBufferImageGranularityBit = 0x00000002,
    LinearAlgorithmBit = 0x00000004,
    AlgorithmMask = LinearAlgorithmBit,
}
[Flags]
public enum DefragmentationFlags : int
{
    FlagAlgorithmFastBit = 0x1,
    FlagAlgorithmBalancedBit = 0x2,
    FlagAlgorithmFullBit = 0x4,
    FlagAlgorithmExtensiveBit = 0x8,
    FlagAlgorithmMask = FlagAlgorithmFastBit | FlagAlgorithmBalancedBit | FlagAlgorithmFullBit | FlagAlgorithmExtensiveBit,
}
public enum DefragmentationMoveOperation : int
{
    Copy = 0,
    Ignore = 1,
    Destroy = 2,
}
[Flags]
public enum VirtualBlockCreateFlags : int
{
    LinearAlgorithmBit = 0x00000001,
    AlgorithmMask = LinearAlgorithmBit,
}
[Flags]
public enum VirtualAllocationCreateFlags : int
{
    UpperAddressBit = AllocationCreateFlags.UpperAddressBit,
    StrategyMinMemoryBit = AllocationCreateFlags.StrategyMinMemoryBit,
    StrategyMinTimeBit = AllocationCreateFlags.StrategyMinTimeBit,
    StrategyMinOffsetBit = AllocationCreateFlags.StrategyMinOffsetBit,
    StrategyMask = AllocationCreateFlags.StrategyMask,
}
public unsafe struct Allocator
{
}
public unsafe struct Pool
{
}
public unsafe struct Allocation
{
}
public unsafe struct DefragmentationContext
{
}
public unsafe struct VirtualAllocation
{
}
public unsafe struct VirtualBlock
{
}
public unsafe struct DeviceMemoryCallbacks
{
    public delegate* unmanaged[Stdcall]<Allocator*, uint, DeviceMemory, ulong, void*, void> PfnAllocate;
    public delegate* unmanaged[Stdcall]<Allocator*, uint, DeviceMemory, ulong, void*, void> PfnFree;
    public void* PUserData;
}
public unsafe struct VulkanFunctions
{
    public delegate* unmanaged[Stdcall]<Instance, byte*, delegate* unmanaged[Stdcall]<void>> VkGetInstanceProcAddr;
    public delegate* unmanaged[Stdcall]<Device, byte*, delegate* unmanaged[Stdcall]<void>> VkGetDeviceProcAddr;
    public delegate* unmanaged[Stdcall]<PhysicalDevice, PhysicalDeviceProperties*, void> VkGetPhysicalDeviceProperties;
    public delegate* unmanaged[Stdcall]<PhysicalDevice, PhysicalDeviceMemoryProperties*, void> VkGetPhysicalDeviceMemoryProperties;
    public delegate* unmanaged[Stdcall]<Device, MemoryAllocateInfo*, AllocationCallbacks*, DeviceMemory*, Result> VkAllocateMemory;
    public delegate* unmanaged[Stdcall]<Device, DeviceMemory, AllocationCallbacks*, void> VkFreeMemory;
    public delegate* unmanaged[Stdcall]<Device, DeviceMemory, ulong, ulong, uint, void**, Result> VkMapMemory;
    public delegate* unmanaged[Stdcall]<Device, DeviceMemory, void> VkUnmapMemory;
    public delegate* unmanaged[Stdcall]<Device, uint, MappedMemoryRange*, Result> VkFlushMappedMemoryRanges;
    public delegate* unmanaged[Stdcall]<Device, uint, MappedMemoryRange*, Result> VkInvalidateMappedMemoryRanges;
    public delegate* unmanaged[Stdcall]<Device, Buffer, DeviceMemory, ulong, Result> VkBindBufferMemory;
    public delegate* unmanaged[Stdcall]<Device, Image, DeviceMemory, ulong, Result> VkBindImageMemory;
    public delegate* unmanaged[Stdcall]<Device, Buffer, MemoryRequirements*, void> VkGetBufferMemoryRequirements;
    public delegate* unmanaged[Stdcall]<Device, Image, MemoryRequirements*, void> VkGetImageMemoryRequirements;
    public delegate* unmanaged[Stdcall]<Device, BufferCreateInfo*, AllocationCallbacks*, Buffer*, Result> VkCreateBuffer;
    public delegate* unmanaged[Stdcall]<Device, Buffer, AllocationCallbacks*, void> VkDestroyBuffer;
    public delegate* unmanaged[Stdcall]<Device, ImageCreateInfo*, AllocationCallbacks*, Image*, Result> VkCreateImage;
    public delegate* unmanaged[Stdcall]<Device, Image, AllocationCallbacks*, void> VkDestroyImage;
    public delegate* unmanaged[Stdcall]<CommandBuffer, Buffer, Buffer, uint, BufferCopy*, void> VkCmdCopyBuffer;
    public delegate* unmanaged[Stdcall]<Device, BufferMemoryRequirementsInfo2*, MemoryRequirements2*, void> VkGetBufferMemoryRequirements2khr;
    public delegate* unmanaged[Stdcall]<Device, ImageMemoryRequirementsInfo2*, MemoryRequirements2*, void> VkGetImageMemoryRequirements2khr;
    public delegate* unmanaged[Stdcall]<Device, uint, BindBufferMemoryInfo*, Result> VkBindBufferMemory2khr;
    public delegate* unmanaged[Stdcall]<Device, uint, BindImageMemoryInfo*, Result> VkBindImageMemory2khr;
    public delegate* unmanaged[Stdcall]<PhysicalDevice, PhysicalDeviceMemoryProperties2*, void> VkGetPhysicalDeviceMemoryProperties2khr;
    public delegate* unmanaged[Stdcall]<Device, DeviceBufferMemoryRequirements*, MemoryRequirements2*, void> VkGetDeviceBufferMemoryRequirements;
    public delegate* unmanaged[Stdcall]<Device, DeviceImageMemoryRequirements*, MemoryRequirements2*, void> VkGetDeviceImageMemoryRequirements;
    public void* VkGetMemoryWin32HandleKhr;
}
public unsafe struct AllocatorCreateInfo
{
    public AllocatorCreateFlags Flags;
    public PhysicalDevice PhysicalDevice;
    public Device Device;
    public ulong PreferredLargeHeapBlockSize;
    public AllocationCallbacks* PAllocationCallbacks;
    public DeviceMemoryCallbacks* PDeviceMemoryCallbacks;
    public ulong* PHeapSizeLimit;
    public VulkanFunctions* PVulkanFunctions;
    public Instance Instance;
    public uint VulkanApiVersion;
    public ExternalMemoryHandleTypeFlags* PTypeExternalMemoryHandleTypes;
}
public unsafe struct AllocatorInfo
{
    public Instance Instance;
    public PhysicalDevice PhysicalDevice;
    public Device Device;
}
public unsafe struct Statistics
{
    public uint BlockCount;
    public uint AllocationCount;
    public ulong BlockBytes;
    public ulong AllocationBytes;
}
public unsafe struct DetailedStatistics
{
    public Statistics Statistics;
    public uint UnusedRangeCount;
    public ulong AllocationSizeMin;
    public ulong AllocationSizeMax;
    public ulong UnusedRangeSizeMin;
    public ulong UnusedRangeSizeMax;
}
public unsafe struct TotalStatistics
{
    public InlineArray32<DetailedStatistics> MemoryType;
    public InlineArray16<DetailedStatistics> MemoryHeap;
    public DetailedStatistics Total;
}
public unsafe struct Budget
{
    public Statistics Statistics;
    public ulong Usage;
    public ulong budget;
}
public unsafe struct AllocationCreateInfo
{
    public AllocationCreateFlags Flags;
    public MemoryUsage Usage;
    public MemoryPropertyFlags RequiredFlags;
    public MemoryPropertyFlags PreferredFlags;
    public uint MemoryTypeBits;
    public Pool* Pool;
    public void* PUserData;
    public float Priority;
}
public unsafe struct PoolCreateInfo
{
    public uint MemoryTypeIndex;
    public PoolCreateFlags Flags;
    public ulong BlockSize;
    public nuint MinBlockCount;
    public nuint MaxBlockCount;
    public float Priority;
    public ulong MinAllocationAlignment;
    public void* PMemoryAllocateNext;
}
public unsafe struct AllocationInfo
{
    public uint MemoryType;
    public DeviceMemory DeviceMemory;
    public ulong Offset;
    public ulong Size;
    public void* PMappedData;
    public void* PUserData;
    public byte* PName;
}
public unsafe struct AllocationInfo2
{
    public AllocationInfo AllocationInfo;
    public ulong BlockSize;
    public uint DedicatedMemory;
}
public unsafe struct DefragmentationInfo
{
    public DefragmentationFlags Flags;
    public Pool* Pool;
    public ulong MaxBytesPerPass;
    public uint MaxAllocationsPerPass;
    public delegate* unmanaged[Stdcall]<void*, uint> PfnBreakCallback;
    public void* PBreakCallbackUserData;
}
public unsafe struct DefragmentationMove
{
    public DefragmentationMoveOperation Operation;
    public Allocation* SrcAllocation;
    public Allocation* DstTmpAllocation;
}
public unsafe struct DefragmentationPassMoveInfo
{
    public uint MoveCount;
    public DefragmentationMove* PMoves;
}
public unsafe struct DefragmentationStats
{
    public ulong BytesMoved;
    public ulong BytesFreed;
    public uint AllocationsMoved;
    public uint DeviceMemoryBlocksFreed;
}
public unsafe struct VirtualBlockCreateInfo
{
    public ulong Size;
    public VirtualBlockCreateFlags Flags;
    public AllocationCallbacks* PAllocationCallbacks;
}
public unsafe struct VirtualAllocationCreateInfo
{
    public ulong Size;
    public ulong Alignment;
    public VirtualAllocationCreateFlags Flags;
    public void* PUserData;
}
public unsafe struct VirtualAllocationInfo
{
    public ulong Offset;
    public ulong Size;
    public void* PUserData;
}
public static unsafe partial class Apis
{
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCreateAllocator", ExactSpelling = true)]
    public static extern Result CreateAllocator(AllocatorCreateInfo* pCreateInfo, Allocator** pAllocator);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaDestroyAllocator", ExactSpelling = true)]
    public static extern void DestroyAllocator(Allocator* allocator);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaGetAllocatorInfo", ExactSpelling = true)]
    public static extern void GetAllocatorInfo(Allocator* allocator, AllocatorInfo* pAllocatorInfo);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaGetPhysicalDeviceProperties", ExactSpelling = true)]
    public static extern void GetPhysicalDeviceProperties(Allocator* allocator, PhysicalDeviceProperties** ppPhysicalDeviceProperties);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaGetMemoryProperties", ExactSpelling = true)]
    public static extern void GetMemoryProperties(Allocator* allocator, PhysicalDeviceMemoryProperties** ppPhysicalDeviceMemoryProperties);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaGetMemoryTypeProperties", ExactSpelling = true)]
    public static extern void GetMemoryTypeProperties(Allocator* allocator, uint memoryTypeIndex, uint* pFlags);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaSetCurrentFrameIndex", ExactSpelling = true)]
    public static extern void SetCurrentFrameIndex(Allocator* allocator, uint frameIndex);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCalculateStatistics", ExactSpelling = true)]
    public static extern void CalculateStatistics(Allocator* allocator, TotalStatistics* pStats);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaGetHeapBudgets", ExactSpelling = true)]
    public static extern void GetHeapBudgets(Allocator* allocator, Budget* pBudgets);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaFindMemoryTypeIndex", ExactSpelling = true)]
    public static extern Result FindMemoryTypeIndex(Allocator* allocator, uint memoryTypeBits, AllocationCreateInfo* pAllocationCreateInfo, uint* pMemoryTypeIndex);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaFindMemoryTypeIndexForBufferInfo", ExactSpelling = true)]
    public static extern Result FindMemoryTypeIndexForBufferInfo(Allocator* allocator, BufferCreateInfo* pBufferCreateInfo, AllocationCreateInfo* pAllocationCreateInfo, uint* pMemoryTypeIndex);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaFindMemoryTypeIndexForImageInfo", ExactSpelling = true)]
    public static extern Result FindMemoryTypeIndexForImageInfo(Allocator* allocator, ImageCreateInfo* pImageCreateInfo, AllocationCreateInfo* pAllocationCreateInfo, uint* pMemoryTypeIndex);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCreatePool", ExactSpelling = true)]
    public static extern Result CreatePool(Allocator* allocator, PoolCreateInfo* pCreateInfo, Pool** pPool);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaDestroyPool", ExactSpelling = true)]
    public static extern void DestroyPool(Allocator* allocator, Pool* pool);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaGetPoolStatistics", ExactSpelling = true)]
    public static extern void GetPoolStatistics(Allocator* allocator, Pool* pool, Statistics* pPoolStats);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCalculatePoolStatistics", ExactSpelling = true)]
    public static extern void CalculatePoolStatistics(Allocator* allocator, Pool* pool, DetailedStatistics* pPoolStats);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCheckPoolCorruption", ExactSpelling = true)]
    public static extern Result CheckPoolCorruption(Allocator* allocator, Pool* pool);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaGetPoolName", ExactSpelling = true)]
    public static extern void GetPoolName(Allocator* allocator, Pool* pool, byte** ppName);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaSetPoolName", ExactSpelling = true)]
    public static extern void SetPoolName(Allocator* allocator, Pool* pool, byte* pName);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaAllocateMemory", ExactSpelling = true)]
    public static extern Result AllocateMemory(Allocator* allocator, MemoryRequirements* pVkMemoryRequirements, AllocationCreateInfo* pCreateInfo, Allocation** pAllocation, AllocationInfo* pAllocationInfo);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaAllocateMemoryPages", ExactSpelling = true)]
    public static extern Result AllocateMemoryPages(Allocator* allocator, MemoryRequirements* pVkMemoryRequirements, AllocationCreateInfo* pCreateInfo, nuint allocationCount, Allocation** pAllocations, AllocationInfo* pAllocationInfo);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaAllocateMemoryForBuffer", ExactSpelling = true)]
    public static extern Result AllocateMemoryForBuffer(Allocator* allocator, Buffer buffer, AllocationCreateInfo* pCreateInfo, Allocation** pAllocation, AllocationInfo* pAllocationInfo);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaAllocateMemoryForImage", ExactSpelling = true)]
    public static extern Result AllocateMemoryForImage(Allocator* allocator, Image image, AllocationCreateInfo* pCreateInfo, Allocation** pAllocation, AllocationInfo* pAllocationInfo);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaFreeMemory", ExactSpelling = true)]
    public static extern void FreeMemory(Allocator* allocator, Allocation* allocation);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaFreeMemoryPages", ExactSpelling = true)]
    public static extern void FreeMemoryPages(Allocator* allocator, nuint allocationCount, Allocation** pAllocations);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaGetAllocationInfo", ExactSpelling = true)]
    public static extern void GetAllocationInfo(Allocator* allocator, Allocation* allocation, AllocationInfo* pAllocationInfo);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaGetAllocationInfo2", ExactSpelling = true)]
    public static extern void GetAllocationInfo2(Allocator* allocator, Allocation* allocation, AllocationInfo2* pAllocationInfo);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaSetAllocationUserData", ExactSpelling = true)]
    public static extern void SetAllocationUserData(Allocator* allocator, Allocation* allocation, void* pUserData);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaSetAllocationName", ExactSpelling = true)]
    public static extern void SetAllocationName(Allocator* allocator, Allocation* allocation, byte* pName);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaGetAllocationMemoryProperties", ExactSpelling = true)]
    public static extern void GetAllocationMemoryProperties(Allocator* allocator, Allocation* allocation, uint* pFlags);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaMapMemory", ExactSpelling = true)]
    public static extern Result MapMemory(Allocator* allocator, Allocation* allocation, void** ppData);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaUnmapMemory", ExactSpelling = true)]
    public static extern void UnmapMemory(Allocator* allocator, Allocation* allocation);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaFlushAllocation", ExactSpelling = true)]
    public static extern Result FlushAllocation(Allocator* allocator, Allocation* allocation, ulong offset, ulong size);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaInvalidateAllocation", ExactSpelling = true)]
    public static extern Result InvalidateAllocation(Allocator* allocator, Allocation* allocation, ulong offset, ulong size);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaFlushAllocations", ExactSpelling = true)]
    public static extern Result FlushAllocations(Allocator* allocator, uint allocationCount, Allocation** allocations, ulong* offsets, ulong* sizes);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaInvalidateAllocations", ExactSpelling = true)]
    public static extern Result InvalidateAllocations(Allocator* allocator, uint allocationCount, Allocation** allocations, ulong* offsets, ulong* sizes);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCopyMemoryToAllocation", ExactSpelling = true)]
    public static extern Result CopyMemoryToAllocation(Allocator* allocator, void* pSrcHostPointer, Allocation* dstAllocation, ulong dstAllocationLocalOffset, ulong size);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCopyAllocationToMemory", ExactSpelling = true)]
    public static extern Result CopyAllocationToMemory(Allocator* allocator, Allocation* srcAllocation, ulong srcAllocationLocalOffset, void* pDstHostPointer, ulong size);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCheckCorruption", ExactSpelling = true)]
    public static extern Result CheckCorruption(Allocator* allocator, uint memoryTypeBits);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaBeginDefragmentation", ExactSpelling = true)]
    public static extern Result BeginDefragmentation(Allocator* allocator, DefragmentationInfo* pInfo, DefragmentationContext** pContext);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaEndDefragmentation", ExactSpelling = true)]
    public static extern void EndDefragmentation(Allocator* allocator, DefragmentationContext* context, DefragmentationStats* pStats);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaBeginDefragmentationPass", ExactSpelling = true)]
    public static extern Result BeginDefragmentationPass(Allocator* allocator, DefragmentationContext* context, DefragmentationPassMoveInfo* pPassInfo);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaEndDefragmentationPass", ExactSpelling = true)]
    public static extern Result EndDefragmentationPass(Allocator* allocator, DefragmentationContext* context, DefragmentationPassMoveInfo* pPassInfo);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaBindBufferMemory", ExactSpelling = true)]
    public static extern Result BindBufferMemory(Allocator* allocator, Allocation* allocation, Buffer buffer);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaBindBufferMemory2", ExactSpelling = true)]
    public static extern Result BindBufferMemory2(Allocator* allocator, Allocation* allocation, ulong allocationLocalOffset, Buffer buffer, void* pNext);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaBindImageMemory", ExactSpelling = true)]
    public static extern Result BindImageMemory(Allocator* allocator, Allocation* allocation, Image image);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaBindImageMemory2", ExactSpelling = true)]
    public static extern Result BindImageMemory2(Allocator* allocator, Allocation* allocation, ulong allocationLocalOffset, Image image, void* pNext);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCreateBuffer", ExactSpelling = true)]
    public static extern Result CreateBuffer(Allocator* allocator, BufferCreateInfo* pBufferCreateInfo, AllocationCreateInfo* pAllocationCreateInfo, Buffer* pBuffer, Allocation** pAllocation, AllocationInfo* pAllocationInfo);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCreateBufferWithAlignment", ExactSpelling = true)]
    public static extern Result CreateBufferWithAlignment(Allocator* allocator, BufferCreateInfo* pBufferCreateInfo, AllocationCreateInfo* pAllocationCreateInfo, ulong minAlignment, Buffer* pBuffer, Allocation** pAllocation, AllocationInfo* pAllocationInfo);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCreateAliasingBuffer", ExactSpelling = true)]
    public static extern Result CreateAliasingBuffer(Allocator* allocator, Allocation* allocation, BufferCreateInfo* pBufferCreateInfo, Buffer* pBuffer);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCreateAliasingBuffer2", ExactSpelling = true)]
    public static extern Result CreateAliasingBuffer2(Allocator* allocator, Allocation* allocation, ulong allocationLocalOffset, BufferCreateInfo* pBufferCreateInfo, Buffer* pBuffer);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaDestroyBuffer", ExactSpelling = true)]
    public static extern void DestroyBuffer(Allocator* allocator, Buffer buffer, Allocation* allocation);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCreateImage", ExactSpelling = true)]
    public static extern Result CreateImage(Allocator* allocator, ImageCreateInfo* pImageCreateInfo, AllocationCreateInfo* pAllocationCreateInfo, Image* pImage, Allocation** pAllocation, AllocationInfo* pAllocationInfo);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCreateAliasingImage", ExactSpelling = true)]
    public static extern Result CreateAliasingImage(Allocator* allocator, Allocation* allocation, ImageCreateInfo* pImageCreateInfo, Image* pImage);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCreateAliasingImage2", ExactSpelling = true)]
    public static extern Result CreateAliasingImage2(Allocator* allocator, Allocation* allocation, ulong allocationLocalOffset, ImageCreateInfo* pImageCreateInfo, Image* pImage);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaDestroyImage", ExactSpelling = true)]
    public static extern void DestroyImage(Allocator* allocator, Image image, Allocation* allocation);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCreateVirtualBlock", ExactSpelling = true)]
    public static extern Result CreateVirtualBlock(VirtualBlockCreateInfo* pCreateInfo, VirtualBlock** pVirtualBlock);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaDestroyVirtualBlock", ExactSpelling = true)]
    public static extern void DestroyVirtualBlock(VirtualBlock* virtualBlock);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaIsVirtualBlockEmpty", ExactSpelling = true)]
    public static extern uint IsVirtualBlockEmpty(VirtualBlock* virtualBlock);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaGetVirtualAllocationInfo", ExactSpelling = true)]
    public static extern void GetVirtualAllocationInfo(VirtualBlock* virtualBlock, VirtualAllocation* allocation, VirtualAllocationInfo* pVirtualAllocInfo);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaVirtualAllocate", ExactSpelling = true)]
    public static extern Result VirtualAllocate(VirtualBlock* virtualBlock, VirtualAllocationCreateInfo* pCreateInfo, VirtualAllocation** pAllocation, ulong* pOffset);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaVirtualFree", ExactSpelling = true)]
    public static extern void VirtualFree(VirtualBlock* virtualBlock, VirtualAllocation* allocation);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaClearVirtualBlock", ExactSpelling = true)]
    public static extern void ClearVirtualBlock(VirtualBlock* virtualBlock);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaSetVirtualAllocationUserData", ExactSpelling = true)]
    public static extern void SetVirtualAllocationUserData(VirtualBlock* virtualBlock, VirtualAllocation* allocation, void* pUserData);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaGetVirtualBlockStatistics", ExactSpelling = true)]
    public static extern void GetVirtualBlockStatistics(VirtualBlock* virtualBlock, Statistics* pStats);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaCalculateVirtualBlockStatistics", ExactSpelling = true)]
    public static extern void CalculateVirtualBlockStatistics(VirtualBlock* virtualBlock, DetailedStatistics* pStats);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaBuildVirtualBlockStatsString", ExactSpelling = true)]
    public static extern void BuildVirtualBlockStatsString(VirtualBlock* virtualBlock, byte** ppStatsString, uint detailedMap);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaFreeVirtualBlockStatsString", ExactSpelling = true)]
    public static extern void FreeVirtualBlockStatsString(VirtualBlock* virtualBlock, byte* pStatsString);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaBuildStatsString", ExactSpelling = true)]
    public static extern void BuildStatsString(Allocator* allocator, byte** ppStatsString, uint detailedMap);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("vma", EntryPoint = "vmaFreeStatsString", ExactSpelling = true)]
    public static extern void FreeStatsString(Allocator* allocator, byte* pStatsString);
}
