# GpuMemoryAllocator

C# bindings for d3d12ma and vma, based on silk.net.

## Build

- Note that you need to clone the submodule.

- Windows
  - Required clang
  - Required .NET 10
  ```powershell
  ./init.ps1
  ./build.ps1
  ```
- Linux
  - Required .NET 10
  ```shell
  ./init.sh
  ./build.sh
  ```

### Sync Version

- Required pwsh

```powershell
./sync.ps1
```
