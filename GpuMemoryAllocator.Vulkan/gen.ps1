ClangSharpPInvokeGenerator -std=c++23 "@gen_on_win.rsp"
dotnet run --project ../VkPostGen/VkPostGen.csproj -- ./binding.xml ./Binding.cs
dotnet format
