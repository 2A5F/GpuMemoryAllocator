using Silk.NET.Core.Native;

namespace D3D12MA;

public unsafe partial struct Allocation : IComVtbl<Allocation>
{
    public void*** AsVtblPtr()
        => (void***) Unsafe.AsPointer(ref Unsafe.AsRef(in this));
}

public unsafe partial struct DefragmentationContext : IComVtbl<DefragmentationContext>
{
    public void*** AsVtblPtr()
        => (void***) Unsafe.AsPointer(ref Unsafe.AsRef(in this));
}

public unsafe partial struct Pool : IComVtbl<Pool>
{
    public void*** AsVtblPtr()
        => (void***) Unsafe.AsPointer(ref Unsafe.AsRef(in this));
}

public unsafe partial struct Allocator : IComVtbl<Allocator>
{
    public void*** AsVtblPtr()
        => (void***) Unsafe.AsPointer(ref Unsafe.AsRef(in this));
}

public unsafe partial struct VirtualBlock : IComVtbl<VirtualBlock>
{
    public void*** AsVtblPtr()
        => (void***) Unsafe.AsPointer(ref Unsafe.AsRef(in this));
}
