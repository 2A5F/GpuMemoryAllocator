#if WIN32
#include "D3D12MemAlloc.h"

void __declspec(dllexport) stub_d3d12ma()
{
    D3D12MA::CreateAllocator(nullptr, nullptr);
}
#endif

#if defined(_WIN32) || defined(_WIN64)
#define DLL_EXPORT __declspec(dllexport)
#elif defined(__GNUC__)
#define DLL_EXPORT __attribute__((visibility("default")))
#else
#define DLL_EXPORT
#endif

#include "vma/vk_mem_alloc.h"

void DLL_EXPORT stub_vma()
{
    vmaCreateAllocator(reinterpret_cast<const VmaAllocatorCreateInfo*>(-1), reinterpret_cast<VmaAllocator*>(-1));
}
