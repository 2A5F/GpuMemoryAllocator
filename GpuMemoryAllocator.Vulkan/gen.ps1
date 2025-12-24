ClangSharpPInvokeGenerator -std=c++23 "@gen_on_win.rsp"
#dotnet run --project ../D3d12PostGen/D3d12PostGen.csproj -- ./binding.xml ./Binding.cs
dotnet format
