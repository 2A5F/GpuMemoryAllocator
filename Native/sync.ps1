param($ThisVersion, $VmaVersion, $D3d12maVersion)

Set-Location -Path $PSScriptRoot

$d3d12ma_ver = "$($D3d12maVersion)-$($ThisVersion)"
$vma_ver = "$($VmaVersion)-$($ThisVersion)"

dotnet tool run t4 "./D3D12MA/D3D12MA.tt" -o "./D3D12MA/D3D12MA.nuspec" -p:Version=$d3d12ma_ver

dotnet tool run t4 "./vma/vma.tt" -o "./vma/vma.nuspec" -p:Version=$vma_ver
