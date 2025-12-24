param($ThisVersion, $VmaVersion, $D3d12maVersion)

Set-Location -Path $PSScriptRoot

$d3d12ma_ver = "$($D3d12maVersion)-$($ThisVersion)"
$vma_ver = "$($VmaVersion)-$($ThisVersion)"

dotnet tool run t4 "./nuspec.tt" -o "./D3D12MA.runtime.win-x64/D3D12MA.runtime.win-x64.nuspec" `
  -p:Version=$d3d12ma_ver -p:LibName="D3D12MA" -p:Runtime="win-x64"  -p:FileName="D3D12MA.dll" -p:FilePath="../../build/Release/D3D12MA.dll"

dotnet tool run t4 "./nuspec.tt" -o "./D3D12MA.runtime.win-arm64/D3D12MA.runtime.win-arm64.nuspec" `
  -p:Version=$d3d12ma_ver -p:LibName="D3D12MA" -p:Runtime="win-arm64"  -p:FileName="D3D12MA.dll" -p:FilePath="../../build/Release/D3D12MA.dll"

dotnet tool run t4 "./nuspec.tt" -o "./vma.runtime.win-x64/vma.runtime.win-x64.nuspec" `
  -p:Version=$vma_ver -p:LibName="vma" -p:Runtime="win-x64"  -p:FileName="vma.dll" -p:FilePath="../../build/vma/Release/vma.dll"

dotnet tool run t4 "./nuspec.tt" -o "./vma.runtime.win-arm64/vma.runtime.win-arm64.nuspec" `
  -p:Version=$vma_ver -p:LibName="vma" -p:Runtime="win-arm64"  -p:FileName="vma.dll" -p:FilePath="../../build/vma/Release/vma.dll"

dotnet tool run t4 "./nuspec.tt" -o "./vma.runtime.linux-x64/vma.runtime.linux-x64.nuspec" `
  -p:Version=$vma_ver -p:LibName="vma" -p:Runtime="linux-x64"  -p:FileName="libvma.so" -p:FilePath="../../build/vma/Release/libvma.so"

dotnet tool run t4 "./nuspec.tt" -o "./vma.runtime.linux-arm64/vma.runtime.linux-arm64.nuspec" `
  -p:Version=$vma_ver -p:LibName="vma" -p:Runtime="linux-arm64"  -p:FileName="libvma.so" -p:FilePath="../../build/vma/Release/libvma.so"

dotnet tool run t4 "./nuspec.tt" -o "./vma.runtime.macos-arm64/vma.runtime.macos-arm64.nuspec" `
  -p:Version=$vma_ver -p:LibName="vma" -p:Runtime="macos-arm64"  -p:FileName="libvma.dylib" -p:FilePath="../../build/vma/Release/libvma.dylib"

dotnet tool run t4 "./D3D12MA/runtime.tt" -o "./D3D12MA/runtime.json" -p:Version=$d3d12ma_ver
dotnet tool run t4 "./D3D12MA/D3D12MA.tt" -o "./D3D12MA/D3D12MA.nuspec" -p:Version=$d3d12ma_ver

dotnet tool run t4 "./vma/runtime.tt" -o "./vma/runtime.json" -p:Version=$vma_ver
dotnet tool run t4 "./vma/vma.tt" -o "./vma/vma.nuspec" -p:Version=$vma_ver
