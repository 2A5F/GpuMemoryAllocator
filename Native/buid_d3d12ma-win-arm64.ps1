Set-Location -Path $PSScriptRoot
dotnet pack ".\D3D12MA.runtime.win-arm64\D3D12MA.runtime.win-arm64.csproj" -o "./packages"
