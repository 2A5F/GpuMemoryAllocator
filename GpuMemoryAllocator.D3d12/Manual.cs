namespace D3D12MA;

public static unsafe partial class Apis
{
    public static int CreateAllocator(AllocatorDesc* pDesc, out ComPtr<Allocator> Allocator)
    {
        Unsafe.SkipInit(out Allocator);
        fixed (Allocator** ppAllocator = Allocator)
        {
            return CreateAllocator(pDesc, ppAllocator);
        }
    }
    public static int CreateVirtualBlock(VirtualBlockDesc* pDesc, out ComPtr<VirtualBlock> VirtualBlock)
    {
        Unsafe.SkipInit(out VirtualBlock);
        fixed (VirtualBlock** ppVirtualBlock = VirtualBlock)
        {
            return CreateVirtualBlock(pDesc, ppVirtualBlock);
        }
    }
}
