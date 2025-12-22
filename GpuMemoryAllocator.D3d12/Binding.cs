namespace D3D12MA;
public unsafe partial struct AllocationCallbacks
{
    public delegate* unmanaged[Cdecl]<nuint, nuint, void*, void*> pAllocate;
    public delegate* unmanaged[Cdecl]<void*, void*, void> pFree;
    public void* pPrivateData;
}
[Flags]
public enum AllocationFlags : int
{
    None = 0,
    Committed = 0x1,
    NeverAllocate = 0x2,
    WithinBudget = 0x4,
    UpperAddress = 0x8,
    CanAlias = 0x10,
    StrategyMinMemory = 0x00010000,
    StrategyMinTime = 0x00020000,
    StrategyMinOffset = 0x0004000,
    StrategyBestFit = StrategyMinMemory,
    StrategyFirstFit = StrategyMinTime,
    StrategyMask = StrategyMinMemory | StrategyMinTime | StrategyMinOffset,
}
public unsafe partial struct AllocationDesc
{
    public AllocationFlags Flags;
    public HeapType HeapType;
    public HeapFlags ExtraHeapFlags;
    public Pool* CustomPool;
    public void* pPrivateData;
}
public unsafe partial struct Statistics
{
    public uint BlockCount;
    public uint AllocationCount;
    public ulong BlockBytes;
    public ulong AllocationBytes;
}
public unsafe partial struct DetailedStatistics
{
    public Statistics Stats;
    public uint UnusedRangeCount;
    public ulong AllocationSizeMin;
    public ulong AllocationSizeMax;
    public ulong UnusedRangeSizeMin;
    public ulong UnusedRangeSizeMax;
}
public unsafe partial struct TotalStatistics
{
    public InlineArray5<DetailedStatistics> HeapType;
    public InlineArray2<DetailedStatistics> MemorySegmentGroup;
    public DetailedStatistics Total;
}
public unsafe partial struct Budget
{
    public Statistics Stats;
    public ulong UsageBytes;
    public ulong BudgetBytes;
}
public unsafe partial struct VirtualAllocation
{
    public ulong AllocHandle;
}
public unsafe partial struct Allocation
{
    internal IUnknownImpl Base;
    internal void* m_Allocator;
    internal ulong m_Size;
    internal ulong m_Alignment;
    internal ID3D12Resource* m_Resource;
    internal void* m_pPrivateData;
    internal char* m_Name;
    internal _Anonymous_e__Union Anonymous;
    internal PackedData m_PackedData;
    [UnscopedRef]
    internal ref _Anonymous_e__Union._m_Committed_e__Struct m_Committed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return ref Anonymous.m_Committed;
        }
    }
    [UnscopedRef]
    internal ref _Anonymous_e__Union._m_Placed_e__Struct m_Placed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return ref Anonymous.m_Placed;
        }
    }
    [UnscopedRef]
    internal ref _Anonymous_e__Union._m_Heap_e__Struct m_Heap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return ref Anonymous.m_Heap;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetOffset()
    {
        return __((Allocation*)Unsafe.AsPointer(ref this));
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?GetOffset@Allocation@D3D12MA@@QEBA_KXZ")]
        static extern ulong __(Allocation* pThis);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetAlignment()
    {
        return m_Alignment;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetSize()
    {
        return m_Size;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ID3D12Resource* GetResource()
    {
        return m_Resource;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetResource(ID3D12Resource* pResource)
    {
        __((Allocation*)Unsafe.AsPointer(ref this), pResource);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?SetResource@Allocation@D3D12MA@@QEAAXPEAUID3D12Resource@@@Z")]
        static extern void __(Allocation* pThis, ID3D12Resource* pResource);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ID3D12Heap* GetHeap()
    {
        return __((Allocation*)Unsafe.AsPointer(ref this));
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?GetHeap@Allocation@D3D12MA@@QEBAPEAUID3D12Heap@@XZ")]
        static extern ID3D12Heap* __(Allocation* pThis);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPrivateData(void* pPrivateData)
    {
        m_pPrivateData = pPrivateData;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* GetPrivateData()
    {
        return m_pPrivateData;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetName(char* Name)
    {
        __((Allocation*)Unsafe.AsPointer(ref this), Name);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?SetName@Allocation@D3D12MA@@QEAAXPEB_W@Z")]
        static extern void __(Allocation* pThis, char* Name);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char* GetName()
    {
        return m_Name;
    }
    private enum Type : int
    {
        TypeCommitted,
        TypePlaced,
        TypeHeap,
        TypeCount,
    }
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe partial struct _Anonymous_e__Union
    {
        [FieldOffset(0)]
        public _m_Committed_e__Struct m_Committed;
        [FieldOffset(0)]
        public _m_Placed_e__Struct m_Placed;
        [FieldOffset(0)]
        public _m_Heap_e__Struct m_Heap;
        public unsafe partial struct _m_Committed_e__Struct
        {
            public void* list;
            public Allocation* prev;
            public Allocation* next;
        }
        public unsafe partial struct _m_Placed_e__Struct
        {
            public ulong allocHandle;
            public void* block;
        }
        public unsafe partial struct _m_Heap_e__Struct
        {
            public void* list;
            public Allocation* prev;
            public Allocation* next;
            public ID3D12Heap* heap;
        }
    }
    internal unsafe partial struct PackedData
    {
        public uint _bitfield1;
        [UnscopedRef]
        internal uint m_Type
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _bitfield1 & 0x3u;
            }
        }
        [UnscopedRef]
        internal uint m_ResourceDimension
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (_bitfield1 >> 2) & 0x7u;
            }
        }
        [UnscopedRef]
        internal uint m_ResourceFlags
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (_bitfield1 >> 5) & 0xFFFFFFu;
            }
        }
        public uint _bitfield2;
        [UnscopedRef]
        internal uint m_TextureLayout
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _bitfield2 & 0x1FFu;
            }
        }
    }
}
[Flags]
public enum DefragmentationFlags : int
{
    AlgorithmFast = 0x1,
    AlgorithmBalanced = 0x2,
    AlgorithmFull = 0x4,
    AlgorithmMask = AlgorithmFast | AlgorithmBalanced | AlgorithmFull,
}
public unsafe partial struct DefragmentationDesc
{
    public DefragmentationFlags Flags;
    public ulong MaxBytesPerPass;
    public uint MaxAllocationsPerPass;
}
public enum DefragmentationMoveOperation : int
{
    Copy = 0,
    Ignore = 1,
    Destroy = 2,
}
public unsafe partial struct DefragmentationMove
{
    public DefragmentationMoveOperation Operation;
    public Allocation* pSrcAllocation;
    public Allocation* pDstTmpAllocation;
}
public unsafe partial struct DefragmentationPassMoveInfo
{
    public uint MoveCount;
    public DefragmentationMove* pMoves;
}
public unsafe partial struct DefragmentationStats
{
    public ulong BytesMoved;
    public ulong BytesFreed;
    public uint AllocationsMoved;
    public uint HeapsFreed;
}
public unsafe partial struct DefragmentationContext
{
    internal IUnknownImpl Base;
    internal void* m_Pimpl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int BeginPass(DefragmentationPassMoveInfo* pPassInfo)
    {
        return __((DefragmentationContext*)Unsafe.AsPointer(ref this), pPassInfo);
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?BeginPass@DefragmentationContext@D3D12MA@@QEAAJPEAUDEFRAGMENTATION_PASS_MOVE_INFO@2@@Z")]
        static extern int __(DefragmentationContext* pThis, DefragmentationPassMoveInfo* pPassInfo);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int EndPass(DefragmentationPassMoveInfo* pPassInfo)
    {
        return __((DefragmentationContext*)Unsafe.AsPointer(ref this), pPassInfo);
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?EndPass@DefragmentationContext@D3D12MA@@QEAAJPEAUDEFRAGMENTATION_PASS_MOVE_INFO@2@@Z")]
        static extern int __(DefragmentationContext* pThis, DefragmentationPassMoveInfo* pPassInfo);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetStats(DefragmentationStats* pStats)
    {
        __((DefragmentationContext*)Unsafe.AsPointer(ref this), pStats);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?GetStats@DefragmentationContext@D3D12MA@@QEAAXPEAUDEFRAGMENTATION_STATS@2@@Z")]
        static extern void __(DefragmentationContext* pThis, DefragmentationStats* pStats);
    }
}
[Flags]
public enum PoolFlags : int
{
    None = 0,
    AlgorithmLinear = 0x1,
    MsaaTexturesAlwaysCommitted = 0x2,
    AlwaysCommitted = 0x4,
    AlgorithmMask = AlgorithmLinear,
}
public unsafe partial struct PoolDesc
{
    public PoolFlags Flags;
    public HeapProperties HeapProperties;
    public HeapFlags HeapFlags;
    public ulong BlockSize;
    public uint MinBlockCount;
    public uint MaxBlockCount;
    public ulong MinAllocationAlignment;
    public ID3D12ProtectedResourceSession* pProtectedSession;
    public ResidencyPriority ResidencyPriority;
}
public unsafe partial struct Pool
{
    internal IUnknownImpl Base;
    internal void* m_Pimpl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PoolDesc GetDesc()
    {
        return __((Pool*)Unsafe.AsPointer(ref this));
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?GetDesc@Pool@D3D12MA@@QEBA?AUPOOL_DESC@2@XZ")]
        static extern PoolDesc __(Pool* pThis);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetStatistics(Statistics* pStats)
    {
        __((Pool*)Unsafe.AsPointer(ref this), pStats);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?GetStatistics@Pool@D3D12MA@@QEAAXPEAUStatistics@2@@Z")]
        static extern void __(Pool* pThis, Statistics* pStats);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CalculateStatistics(DetailedStatistics* pStats)
    {
        __((Pool*)Unsafe.AsPointer(ref this), pStats);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?CalculateStatistics@Pool@D3D12MA@@QEAAXPEAUDetailedStatistics@2@@Z")]
        static extern void __(Pool* pThis, DetailedStatistics* pStats);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetName(char* Name)
    {
        __((Pool*)Unsafe.AsPointer(ref this), Name);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?SetName@Pool@D3D12MA@@QEAAXPEB_W@Z")]
        static extern void __(Pool* pThis, char* Name);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char* GetName()
    {
        return __((Pool*)Unsafe.AsPointer(ref this));
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?GetName@Pool@D3D12MA@@QEBAPEB_WXZ")]
        static extern char* __(Pool* pThis);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int BeginDefragmentation(DefragmentationDesc* pDesc, DefragmentationContext** ppContext)
    {
        return __((Pool*)Unsafe.AsPointer(ref this), pDesc, ppContext);
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?BeginDefragmentation@Pool@D3D12MA@@QEAAJPEBUDEFRAGMENTATION_DESC@2@PEAPEAVDefragmentationContext@2@@Z")]
        static extern int __(Pool* pThis, DefragmentationDesc* pDesc, DefragmentationContext** ppContext);
    }
}
[Flags]
public enum AllocatorFlags : int
{
    None = 0,
    SingleThreaded = 0x1,
    AlwaysCommitted = 0x2,
    DefaultPoolsNotZeroed = 0x4,
    MsaaTexturesAlwaysCommitted = 0x8,
    DontPreferSmallBuffersCommitted = 0x10,
}
public unsafe partial struct AllocatorDesc
{
    public AllocatorFlags Flags;
    public ID3D12Device* pDevice;
    public ulong PreferredBlockSize;
    public AllocationCallbacks* pAllocationCallbacks;
    public IDXGIAdapter* pAdapter;
}
public unsafe partial struct Allocator
{
    internal IUnknownImpl Base;
    internal void* m_Pimpl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FeatureDataD3D12Options* GetD3D12Options()
    {
        return __((Allocator*)Unsafe.AsPointer(ref this));
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?GetD3D12Options@Allocator@D3D12MA@@QEBAAEBUD3D12_FEATURE_DATA_D3D12_OPTIONS@@XZ")]
        static extern FeatureDataD3D12Options* __(Allocator* pThis);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IsUMA()
    {
        return __((Allocator*)Unsafe.AsPointer(ref this));
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?IsUMA@Allocator@D3D12MA@@QEBAHXZ")]
        static extern int __(Allocator* pThis);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IsCacheCoherentUMA()
    {
        return __((Allocator*)Unsafe.AsPointer(ref this));
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?IsCacheCoherentUMA@Allocator@D3D12MA@@QEBAHXZ")]
        static extern int __(Allocator* pThis);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IsGPUUploadHeapSupported()
    {
        return __((Allocator*)Unsafe.AsPointer(ref this));
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?IsGPUUploadHeapSupported@Allocator@D3D12MA@@QEBAHXZ")]
        static extern int __(Allocator* pThis);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetMemoryCapacity(uint memorySegmentGroup)
    {
        return __((Allocator*)Unsafe.AsPointer(ref this), memorySegmentGroup);
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?GetMemoryCapacity@Allocator@D3D12MA@@QEBA_KI@Z")]
        static extern ulong __(Allocator* pThis, uint memorySegmentGroup);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreateResource(AllocationDesc* pAllocDesc, ResourceDesc* pResourceDesc, ResourceStates InitialResourceState, ClearValue* pOptimizedClearValue, Allocation** ppAllocation, Guid* riidResource, void** ppvResource)
    {
        return __((Allocator*)Unsafe.AsPointer(ref this), pAllocDesc, pResourceDesc, InitialResourceState, pOptimizedClearValue, ppAllocation, riidResource, ppvResource);
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?CreateResource@Allocator@D3D12MA@@QEAAJPEBUALLOCATION_DESC@2@PEBUD3D12_RESOURCE_DESC@@W4D3D12_RESOURCE_STATES@@PEBUD3D12_CLEAR_VALUE@@PEAPEAVAllocation@2@AEBU_GUID@@PEAPEAX@Z")]
        static extern int __(Allocator* pThis, AllocationDesc* pAllocDesc, ResourceDesc* pResourceDesc, ResourceStates InitialResourceState, ClearValue* pOptimizedClearValue, Allocation** ppAllocation, Guid* riidResource, void** ppvResource);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreateResource2(AllocationDesc* pAllocDesc, ResourceDesc1* pResourceDesc, ResourceStates InitialResourceState, ClearValue* pOptimizedClearValue, Allocation** ppAllocation, Guid* riidResource, void** ppvResource)
    {
        return __((Allocator*)Unsafe.AsPointer(ref this), pAllocDesc, pResourceDesc, InitialResourceState, pOptimizedClearValue, ppAllocation, riidResource, ppvResource);
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?CreateResource2@Allocator@D3D12MA@@QEAAJPEBUALLOCATION_DESC@2@PEBUD3D12_RESOURCE_DESC1@@W4D3D12_RESOURCE_STATES@@PEBUD3D12_CLEAR_VALUE@@PEAPEAVAllocation@2@AEBU_GUID@@PEAPEAX@Z")]
        static extern int __(Allocator* pThis, AllocationDesc* pAllocDesc, ResourceDesc1* pResourceDesc, ResourceStates InitialResourceState, ClearValue* pOptimizedClearValue, Allocation** ppAllocation, Guid* riidResource, void** ppvResource);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreateResource3(AllocationDesc* pAllocDesc, ResourceDesc1* pResourceDesc, BarrierLayout InitialLayout, ClearValue* pOptimizedClearValue, uint NumCastableFormats, Format* pCastableFormats, Allocation** ppAllocation, Guid* riidResource, void** ppvResource)
    {
        return __((Allocator*)Unsafe.AsPointer(ref this), pAllocDesc, pResourceDesc, InitialLayout, pOptimizedClearValue, NumCastableFormats, pCastableFormats, ppAllocation, riidResource, ppvResource);
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?CreateResource3@Allocator@D3D12MA@@QEAAJPEBUALLOCATION_DESC@2@PEBUD3D12_RESOURCE_DESC1@@W4D3D12_BARRIER_LAYOUT@@PEBUD3D12_CLEAR_VALUE@@IPEBW4DXGI_FORMAT@@PEAPEAVAllocation@2@AEBU_GUID@@PEAPEAX@Z")]
        static extern int __(Allocator* pThis, AllocationDesc* pAllocDesc, ResourceDesc1* pResourceDesc, BarrierLayout InitialLayout, ClearValue* pOptimizedClearValue, uint NumCastableFormats, Format* pCastableFormats, Allocation** ppAllocation, Guid* riidResource, void** ppvResource);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int AllocateMemory(AllocationDesc* pAllocDesc, ResourceAllocationInfo* pAllocInfo, Allocation** ppAllocation)
    {
        return __((Allocator*)Unsafe.AsPointer(ref this), pAllocDesc, pAllocInfo, ppAllocation);
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?AllocateMemory@Allocator@D3D12MA@@QEAAJPEBUALLOCATION_DESC@2@PEBUD3D12_RESOURCE_ALLOCATION_INFO@@PEAPEAVAllocation@2@@Z")]
        static extern int __(Allocator* pThis, AllocationDesc* pAllocDesc, ResourceAllocationInfo* pAllocInfo, Allocation** ppAllocation);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreateAliasingResource(Allocation* pAllocation, ulong AllocationLocalOffset, ResourceDesc* pResourceDesc, ResourceStates InitialResourceState, ClearValue* pOptimizedClearValue, Guid* riidResource, void** ppvResource)
    {
        return __((Allocator*)Unsafe.AsPointer(ref this), pAllocation, AllocationLocalOffset, pResourceDesc, InitialResourceState, pOptimizedClearValue, riidResource, ppvResource);
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?CreateAliasingResource@Allocator@D3D12MA@@QEAAJPEAVAllocation@2@_KPEBUD3D12_RESOURCE_DESC@@W4D3D12_RESOURCE_STATES@@PEBUD3D12_CLEAR_VALUE@@AEBU_GUID@@PEAPEAX@Z")]
        static extern int __(Allocator* pThis, Allocation* pAllocation, ulong AllocationLocalOffset, ResourceDesc* pResourceDesc, ResourceStates InitialResourceState, ClearValue* pOptimizedClearValue, Guid* riidResource, void** ppvResource);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreateAliasingResource1(Allocation* pAllocation, ulong AllocationLocalOffset, ResourceDesc1* pResourceDesc, ResourceStates InitialResourceState, ClearValue* pOptimizedClearValue, Guid* riidResource, void** ppvResource)
    {
        return __((Allocator*)Unsafe.AsPointer(ref this), pAllocation, AllocationLocalOffset, pResourceDesc, InitialResourceState, pOptimizedClearValue, riidResource, ppvResource);
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?CreateAliasingResource1@Allocator@D3D12MA@@QEAAJPEAVAllocation@2@_KPEBUD3D12_RESOURCE_DESC1@@W4D3D12_RESOURCE_STATES@@PEBUD3D12_CLEAR_VALUE@@AEBU_GUID@@PEAPEAX@Z")]
        static extern int __(Allocator* pThis, Allocation* pAllocation, ulong AllocationLocalOffset, ResourceDesc1* pResourceDesc, ResourceStates InitialResourceState, ClearValue* pOptimizedClearValue, Guid* riidResource, void** ppvResource);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreateAliasingResource2(Allocation* pAllocation, ulong AllocationLocalOffset, ResourceDesc1* pResourceDesc, BarrierLayout InitialLayout, ClearValue* pOptimizedClearValue, uint NumCastableFormats, Format* pCastableFormats, Guid* riidResource, void** ppvResource)
    {
        return __((Allocator*)Unsafe.AsPointer(ref this), pAllocation, AllocationLocalOffset, pResourceDesc, InitialLayout, pOptimizedClearValue, NumCastableFormats, pCastableFormats, riidResource, ppvResource);
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?CreateAliasingResource2@Allocator@D3D12MA@@QEAAJPEAVAllocation@2@_KPEBUD3D12_RESOURCE_DESC1@@W4D3D12_BARRIER_LAYOUT@@PEBUD3D12_CLEAR_VALUE@@IPEBW4DXGI_FORMAT@@AEBU_GUID@@PEAPEAX@Z")]
        static extern int __(Allocator* pThis, Allocation* pAllocation, ulong AllocationLocalOffset, ResourceDesc1* pResourceDesc, BarrierLayout InitialLayout, ClearValue* pOptimizedClearValue, uint NumCastableFormats, Format* pCastableFormats, Guid* riidResource, void** ppvResource);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreatePool(PoolDesc* pPoolDesc, Pool** ppPool)
    {
        return __((Allocator*)Unsafe.AsPointer(ref this), pPoolDesc, ppPool);
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?CreatePool@Allocator@D3D12MA@@QEAAJPEBUPOOL_DESC@2@PEAPEAVPool@2@@Z")]
        static extern int __(Allocator* pThis, PoolDesc* pPoolDesc, Pool** ppPool);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetCurrentFrameIndex(uint frameIndex)
    {
        __((Allocator*)Unsafe.AsPointer(ref this), frameIndex);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?SetCurrentFrameIndex@Allocator@D3D12MA@@QEAAXI@Z")]
        static extern void __(Allocator* pThis, uint frameIndex);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetBudget(Budget* pLocalBudget, Budget* pNonLocalBudget)
    {
        __((Allocator*)Unsafe.AsPointer(ref this), pLocalBudget, pNonLocalBudget);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?GetBudget@Allocator@D3D12MA@@QEAAXPEAUBudget@2@0@Z")]
        static extern void __(Allocator* pThis, Budget* pLocalBudget, Budget* pNonLocalBudget);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CalculateStatistics(TotalStatistics* pStats)
    {
        __((Allocator*)Unsafe.AsPointer(ref this), pStats);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?CalculateStatistics@Allocator@D3D12MA@@QEAAXPEAUTotalStatistics@2@@Z")]
        static extern void __(Allocator* pThis, TotalStatistics* pStats);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BuildStatsString(char** ppStatsString, int DetailedMap)
    {
        __((Allocator*)Unsafe.AsPointer(ref this), ppStatsString, DetailedMap);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?BuildStatsString@Allocator@D3D12MA@@QEBAXPEAPEA_WH@Z")]
        static extern void __(Allocator* pThis, char** ppStatsString, int DetailedMap);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FreeStatsString(char* pStatsString)
    {
        __((Allocator*)Unsafe.AsPointer(ref this), pStatsString);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?FreeStatsString@Allocator@D3D12MA@@QEBAXPEA_W@Z")]
        static extern void __(Allocator* pThis, char* pStatsString);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BeginDefragmentation(DefragmentationDesc* pDesc, DefragmentationContext** ppContext)
    {
        __((Allocator*)Unsafe.AsPointer(ref this), pDesc, ppContext);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?BeginDefragmentation@Allocator@D3D12MA@@QEAAXPEBUDEFRAGMENTATION_DESC@2@PEAPEAVDefragmentationContext@2@@Z")]
        static extern void __(Allocator* pThis, DefragmentationDesc* pDesc, DefragmentationContext** ppContext);
    }
}
[Flags]
public enum VirtualBlockFlags : int
{
    None = 0,
    AlgorithmLinear = PoolFlags.AlgorithmLinear,
    AlgorithmMask = PoolFlags.AlgorithmMask,
}
public unsafe partial struct VirtualBlockDesc
{
    public VirtualBlockFlags Flags;
    public ulong Size;
    public AllocationCallbacks* pAllocationCallbacks;
}
[Flags]
public enum VirtualAllocationFlags : int
{
    None = 0,
    UpperAddress = AllocationFlags.UpperAddress,
    StrategyMinMemory = AllocationFlags.StrategyMinMemory,
    StrategyMinTime = AllocationFlags.StrategyMinTime,
    StrategyMinOffset = AllocationFlags.StrategyMinOffset,
    StrategyMask = AllocationFlags.StrategyMask,
}
public unsafe partial struct VirtualAllocationDesc
{
    public VirtualAllocationFlags Flags;
    public ulong Size;
    public ulong Alignment;
    public void* pPrivateData;
}
public unsafe partial struct VirtualAllocationInfo
{
    public ulong Offset;
    public ulong Size;
    public void* pPrivateData;
}
public unsafe partial struct VirtualBlock
{
    internal IUnknownImpl Base;
    internal void* m_Pimpl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IsEmpty()
    {
        return __((VirtualBlock*)Unsafe.AsPointer(ref this));
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?IsEmpty@VirtualBlock@D3D12MA@@QEBAHXZ")]
        static extern int __(VirtualBlock* pThis);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetAllocationInfo(VirtualAllocation allocation, VirtualAllocationInfo* pInfo)
    {
        __((VirtualBlock*)Unsafe.AsPointer(ref this), allocation, pInfo);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?GetAllocationInfo@VirtualBlock@D3D12MA@@QEBAXUVirtualAllocation@2@PEAUVIRTUAL_ALLOCATION_INFO@2@@Z")]
        static extern void __(VirtualBlock* pThis, VirtualAllocation allocation, VirtualAllocationInfo* pInfo);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Allocate(VirtualAllocationDesc* pDesc, VirtualAllocation* pAllocation, ulong* pOffset)
    {
        return __((VirtualBlock*)Unsafe.AsPointer(ref this), pDesc, pAllocation, pOffset);
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?Allocate@VirtualBlock@D3D12MA@@QEAAJPEBUVIRTUAL_ALLOCATION_DESC@2@PEAUVirtualAllocation@2@PEA_K@Z")]
        static extern int __(VirtualBlock* pThis, VirtualAllocationDesc* pDesc, VirtualAllocation* pAllocation, ulong* pOffset);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FreeAllocation(VirtualAllocation allocation)
    {
        __((VirtualBlock*)Unsafe.AsPointer(ref this), allocation);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?FreeAllocation@VirtualBlock@D3D12MA@@QEAAXUVirtualAllocation@2@@Z")]
        static extern void __(VirtualBlock* pThis, VirtualAllocation allocation);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        __((VirtualBlock*)Unsafe.AsPointer(ref this));
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?Clear@VirtualBlock@D3D12MA@@QEAAXXZ")]
        static extern void __(VirtualBlock* pThis);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetAllocationPrivateData(VirtualAllocation allocation, void* pPrivateData)
    {
        __((VirtualBlock*)Unsafe.AsPointer(ref this), allocation, pPrivateData);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?SetAllocationPrivateData@VirtualBlock@D3D12MA@@QEAAXUVirtualAllocation@2@PEAX@Z")]
        static extern void __(VirtualBlock* pThis, VirtualAllocation allocation, void* pPrivateData);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetStatistics(Statistics* pStats)
    {
        __((VirtualBlock*)Unsafe.AsPointer(ref this), pStats);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?GetStatistics@VirtualBlock@D3D12MA@@QEBAXPEAUStatistics@2@@Z")]
        static extern void __(VirtualBlock* pThis, Statistics* pStats);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CalculateStatistics(DetailedStatistics* pStats)
    {
        __((VirtualBlock*)Unsafe.AsPointer(ref this), pStats);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?CalculateStatistics@VirtualBlock@D3D12MA@@QEBAXPEAUDetailedStatistics@2@@Z")]
        static extern void __(VirtualBlock* pThis, DetailedStatistics* pStats);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BuildStatsString(char** ppStatsString)
    {
        __((VirtualBlock*)Unsafe.AsPointer(ref this), ppStatsString);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?BuildStatsString@VirtualBlock@D3D12MA@@QEBAXPEAPEA_W@Z")]
        static extern void __(VirtualBlock* pThis, char** ppStatsString);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FreeStatsString(char* pStatsString)
    {
        __((VirtualBlock*)Unsafe.AsPointer(ref this), pStatsString);
        return;
        [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
        [DllImport("D3D12MA", EntryPoint = "?FreeStatsString@VirtualBlock@D3D12MA@@QEBAXPEA_W@Z")]
        static extern void __(VirtualBlock* pThis, char* pStatsString);
    }
}
public static unsafe partial class Apis
{
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("D3D12MA", EntryPoint = "?CreateAllocator@D3D12MA@@YAJPEBUALLOCATOR_DESC@1@PEAPEAVAllocator@1@@Z", ExactSpelling = true)]
    public static extern int CreateAllocator(AllocatorDesc* pDesc, Allocator** ppAllocator);
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DllImport("D3D12MA", EntryPoint = "?CreateVirtualBlock@D3D12MA@@YAJPEBUVIRTUAL_BLOCK_DESC@1@PEAPEAVVirtualBlock@1@@Z", ExactSpelling = true)]
    public static extern int CreateVirtualBlock(VirtualBlockDesc* pDesc, VirtualBlock** ppVirtualBlock);
}
public unsafe partial struct Allocation : IComVtbl<Allocation>, IComVtbl<IUnknown>
{
    public void*** AsVtblPtr() => (void***) Unsafe.AsPointer(ref Unsafe.AsRef(in this));
}
public unsafe partial struct DefragmentationContext : IComVtbl<DefragmentationContext>, IComVtbl<IUnknown>
{
    public void*** AsVtblPtr() => (void***) Unsafe.AsPointer(ref Unsafe.AsRef(in this));
}
public unsafe partial struct Pool : IComVtbl<Pool>, IComVtbl<IUnknown>
{
    public void*** AsVtblPtr() => (void***) Unsafe.AsPointer(ref Unsafe.AsRef(in this));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int BeginDefragmentation(DefragmentationDesc* pDesc, out ComPtr<DefragmentationContext> Context)
    {
        Unsafe.SkipInit(out Context);
        fixed (DefragmentationContext** ppContext = Context)
        {
            return BeginDefragmentation(pDesc, ppContext);
        }
    }
}
public unsafe partial struct Allocator : IComVtbl<Allocator>, IComVtbl<IUnknown>
{
    public void*** AsVtblPtr() => (void***) Unsafe.AsPointer(ref Unsafe.AsRef(in this));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreateResource<T0>(AllocationDesc* pAllocDesc, ResourceDesc* pResourceDesc, ResourceStates InitialResourceState, ClearValue* pOptimizedClearValue, out ComPtr<Allocation> Allocation, out ComPtr<T0> Resource)
        where T0: unmanaged, IComVtbl<T0>
    {
        Unsafe.SkipInit(out Allocation);
        Unsafe.SkipInit(out Resource);
        fixed (Allocation** ppAllocation = Allocation)
        fixed (T0** ppvResource = Resource)
        {
            return CreateResource(pAllocDesc, pResourceDesc, InitialResourceState, pOptimizedClearValue, ppAllocation, SilkMarshal.GuidPtrOf<T0>(), (void**)ppvResource);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreateResource2<T0>(AllocationDesc* pAllocDesc, ResourceDesc1* pResourceDesc, ResourceStates InitialResourceState, ClearValue* pOptimizedClearValue, out ComPtr<Allocation> Allocation, out ComPtr<T0> Resource)
        where T0: unmanaged, IComVtbl<T0>
    {
        Unsafe.SkipInit(out Allocation);
        Unsafe.SkipInit(out Resource);
        fixed (Allocation** ppAllocation = Allocation)
        fixed (T0** ppvResource = Resource)
        {
            return CreateResource2(pAllocDesc, pResourceDesc, InitialResourceState, pOptimizedClearValue, ppAllocation, SilkMarshal.GuidPtrOf<T0>(), (void**)ppvResource);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreateResource3<T0>(AllocationDesc* pAllocDesc, ResourceDesc1* pResourceDesc, BarrierLayout InitialLayout, ClearValue* pOptimizedClearValue, uint NumCastableFormats, Format* pCastableFormats, out ComPtr<Allocation> Allocation, out ComPtr<T0> Resource)
        where T0: unmanaged, IComVtbl<T0>
    {
        Unsafe.SkipInit(out Allocation);
        Unsafe.SkipInit(out Resource);
        fixed (Allocation** ppAllocation = Allocation)
        fixed (T0** ppvResource = Resource)
        {
            return CreateResource3(pAllocDesc, pResourceDesc, InitialLayout, pOptimizedClearValue, NumCastableFormats, pCastableFormats, ppAllocation, SilkMarshal.GuidPtrOf<T0>(), (void**)ppvResource);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int AllocateMemory(AllocationDesc* pAllocDesc, ResourceAllocationInfo* pAllocInfo, out ComPtr<Allocation> Allocation)
    {
        Unsafe.SkipInit(out Allocation);
        fixed (Allocation** ppAllocation = Allocation)
        {
            return AllocateMemory(pAllocDesc, pAllocInfo, ppAllocation);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreateAliasingResource<T0>(Allocation* pAllocation, ulong AllocationLocalOffset, ResourceDesc* pResourceDesc, ResourceStates InitialResourceState, ClearValue* pOptimizedClearValue, out ComPtr<T0> Resource)
        where T0: unmanaged, IComVtbl<T0>
    {
        Unsafe.SkipInit(out Resource);
        fixed (T0** ppvResource = Resource)
        {
            return CreateAliasingResource(pAllocation, AllocationLocalOffset, pResourceDesc, InitialResourceState, pOptimizedClearValue, SilkMarshal.GuidPtrOf<T0>(), (void**)ppvResource);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreateAliasingResource1<T0>(Allocation* pAllocation, ulong AllocationLocalOffset, ResourceDesc1* pResourceDesc, ResourceStates InitialResourceState, ClearValue* pOptimizedClearValue, out ComPtr<T0> Resource)
        where T0: unmanaged, IComVtbl<T0>
    {
        Unsafe.SkipInit(out Resource);
        fixed (T0** ppvResource = Resource)
        {
            return CreateAliasingResource1(pAllocation, AllocationLocalOffset, pResourceDesc, InitialResourceState, pOptimizedClearValue, SilkMarshal.GuidPtrOf<T0>(), (void**)ppvResource);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreateAliasingResource2<T0>(Allocation* pAllocation, ulong AllocationLocalOffset, ResourceDesc1* pResourceDesc, BarrierLayout InitialLayout, ClearValue* pOptimizedClearValue, uint NumCastableFormats, Format* pCastableFormats, out ComPtr<T0> Resource)
        where T0: unmanaged, IComVtbl<T0>
    {
        Unsafe.SkipInit(out Resource);
        fixed (T0** ppvResource = Resource)
        {
            return CreateAliasingResource2(pAllocation, AllocationLocalOffset, pResourceDesc, InitialLayout, pOptimizedClearValue, NumCastableFormats, pCastableFormats, SilkMarshal.GuidPtrOf<T0>(), (void**)ppvResource);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreatePool(PoolDesc* pPoolDesc, out ComPtr<Pool> Pool)
    {
        Unsafe.SkipInit(out Pool);
        fixed (Pool** ppPool = Pool)
        {
            return CreatePool(pPoolDesc, ppPool);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BeginDefragmentation(DefragmentationDesc* pDesc, out ComPtr<DefragmentationContext> Context)
    {
        Unsafe.SkipInit(out Context);
        fixed (DefragmentationContext** ppContext = Context)
        {
            BeginDefragmentation(pDesc, ppContext);
        }
    }
}
public unsafe partial struct VirtualBlock : IComVtbl<VirtualBlock>, IComVtbl<IUnknown>
{
    public void*** AsVtblPtr() => (void***) Unsafe.AsPointer(ref Unsafe.AsRef(in this));
}
