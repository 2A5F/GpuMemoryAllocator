Set-Location -Path $PSScriptRoot
dotnet pack ".\vma.runtime.win-x64\vma.runtime.win-x64.csproj" -o "./packages"
