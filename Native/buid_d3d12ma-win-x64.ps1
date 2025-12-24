Set-Location -Path $PSScriptRoot
dotnet pack ".\D3D12MA.runtime.win-x64\D3D12MA.runtime.win-x64.csproj" -o "./packages"
