# GpuMemoryAllocator

[![Build](https://github.com/2A5F/GpuMemoryAllocator/actions/workflows/build.yml/badge.svg)](https://github.com/2A5F/GpuMemoryAllocator/actions/workflows/build.yml)

C# bindings for d3d12ma and vma, based on silk.net.

*Since VMA and D3D12MA are not updated frequently, they are not automatically updated. If they become outdated, please
submit an issue to remind us.*

## Packages

- [![Nuget](https://img.shields.io/nuget/v/GpuMemoryAllocator.D3d12) <br/> GpuMemoryAllocator.D3d12](https://www.nuget.org/packages/GpuMemoryAllocator.D3d12/)  
  The D3D12MA binding

    - [![Nuget](https://img.shields.io/nuget/v/GpuMemoryAllocator.D3D12MA) <br/> GpuMemoryAllocator.D3D12MA](https://www.nuget.org/packages/GpuMemoryAllocator.D3D12MA/)  
      The D3D12MA native build

- [![Nuget](https://img.shields.io/nuget/v/GpuMemoryAllocator.Vulkan) <br/> GpuMemoryAllocator.Vulkan](https://www.nuget.org/packages/GpuMemoryAllocator.Vulkan/)  
  The vma binding

    - [![Nuget](https://img.shields.io/nuget/v/GpuMemoryAllocator.vma) <br/> GpuMemoryAllocator.vma](https://www.nuget.org/packages/GpuMemoryAllocator.vma/)  
      The vma native build

## Build

- Required .NET 10
- Required cmake
- Required ninja
- Required [vcpkg](https://github.com/microsoft/vcpkg)

- Windows
    - Required clang
  ```powershell
  ./init.ps1
  ./build.ps1
  ```
- Linux
  ```shell
  ./init.sh
  ./build.sh
  ```

### Sync Version

- Required pwsh

```powershell
./sync.ps1
```

## Links

[D3D12MemoryAllocator](https://github.com/GPUOpen-LibrariesAndSDKs/D3D12MemoryAllocator)  
[VulkanMemoryAllocator](https://github.com/GPUOpen-LibrariesAndSDKs/VulkanMemoryAllocator)  
