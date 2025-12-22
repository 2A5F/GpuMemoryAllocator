#if defined(WIN32)
#include "vma/vk_mem_alloc.h"
#include "D3D12MemAlloc.h"

void __declspec(dllexport) stub_d3d12ma()
{
    D3D12MA::CreateAllocator(nullptr, nullptr);
}
#endif

#if defined(WIN32)
#define DLL_EXPORT __declspec(dllexport)
#elif defined(__GNUC__)
#define DLL_EXPORT __attribute__((visibility("default")))
#else
#define DLL_EXPORT
#endif

#if !defined(WIN32)
#include "vk_mem_alloc.h"
#endif

void DLL_EXPORT stub_vma()
{
    vmaCreateAllocator(reinterpret_cast<const VmaAllocatorCreateInfo*>(-1), reinterpret_cast<VmaAllocator*>(-1));
}
