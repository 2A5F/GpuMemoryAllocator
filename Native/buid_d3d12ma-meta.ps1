Set-Location -Path $PSScriptRoot
dotnet pack ".\D3D12MA\D3D12MA.csproj" -o "./packages"
