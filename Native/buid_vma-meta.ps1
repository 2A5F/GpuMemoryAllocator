Set-Location -Path $PSScriptRoot
dotnet pack ".\vma\vma.csproj" -o "./packages"
