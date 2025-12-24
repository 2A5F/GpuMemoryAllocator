Set-Location -Path $PSScriptRoot
dotnet pack ".\vma.runtime.macos-arm64\vma.runtime.macos-arm64.csproj" -o "./packages"
