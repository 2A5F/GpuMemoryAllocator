using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CaseConverter;

var input_path = args[0];
var output_path = args[1];

var doc = new XmlDocument();
doc.Load(input_path);

var root = doc.DocumentElement!.FirstChild!;
var code = new StringBuilder();

code.AppendLine($"namespace D3D12MA;");

foreach (XmlNode item in root.ChildNodes)
{
    GenItem(code, item, "", true);
}

File.WriteAllText(output_path, code.ToString());

return;

static void GenItem(StringBuilder code, XmlNode item, string tab, bool root)
{
    switch (item.Name)
    {
        case "struct":
        {
            var access = item.Attributes!["access"]!.Value;
            if (access != "public") access = "internal";
            var raw_name = item.Attributes!["name"]!.Value;
            var name = raw_name;
            if (name.EndsWith("FixedBuffer")) return;
            var layout = item.Attributes["layout"]?.Value;
            name = UpperSnakeCase().Replace(name, match => match.Value.ToPascalCase());
            var is_explicit = layout is "Explicit";
            if (is_explicit) code.AppendLine($"{tab}[StructLayout(LayoutKind.Explicit)]");
            code.AppendLine($"{tab}{access} unsafe partial struct {name}");
            code.AppendLine($"{tab}{{");
            foreach (XmlNode member in item.ChildNodes)
            {
                if (member.Name == "field")
                {
                    var field_access = member.Attributes!["access"]!.Value;
                    if (field_access != "public") field_access = "internal";
                    var field_name = member.Attributes!["name"]!.Value;
                    var typ_node = member["type"]!;
                    var typ = typ_node.InnerText;
                    typ = UpperSnakeCase().Replace(typ, match => match.Value.ToPascalCase());
                    if (typ == "IUnknownImpl") field_access = "internal";
                    var count = typ_node.Attributes["count"]?.Value;
                    if (count is not null) typ = $"InlineArray{count}<{typ}>";
                    if (member["get"] is { } get)
                    {
                        code.AppendLine($"{tab}    [UnscopedRef]");
                        code.AppendLine($"{tab}    {field_access} {typ} {field_name}");
                        code.AppendLine($"{tab}    {{");
                        code.AppendLine($"{tab}        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
                        code.AppendLine($"{tab}        get");
                        code.AppendLine($"{tab}        {{");
                        code.AppendLine($"{tab}            {get["code"]!.InnerText}");
                        code.AppendLine($"{tab}        }}");
                        code.AppendLine($"{tab}    }}");
                    }
                    else
                    {
                        if (is_explicit)
                        {
                            var offset = member.Attributes!["offset"]!.Value;
                            code.AppendLine($"{tab}    [FieldOffset({offset})]");
                        }
                        code.AppendLine($"{tab}    {field_access} {typ} {field_name};");
                    }
                }
                else if (member.Name == "function")
                {
                    if (!root) continue;
                    if (access != "public") continue;
                    var method_access = member.Attributes!["access"]!.Value;
                    if (method_access != "public") continue;
                    var method_name = member.Attributes!["name"]!.Value;
                    if (method_name == raw_name) continue;
                    var lib = member.Attributes!["lib"]?.Value;
                    var convention = member.Attributes!["convention"]?.Value.ToLower().FirstCharToUpperCase();
                    var entrypoint = member.Attributes!["entrypoint"]?.Value;
                    var ret_typ_node = member["type"];
                    var ret_type = ret_typ_node?.InnerText!;
                    ret_type = UpperSnakeCase().Replace(ret_type, match => match.Value.ToPascalCase());
                    var is_static = true;
                    foreach (XmlNode param in member.ChildNodes)
                    {
                        if (param.Name != "param") continue;
                        if (param.Attributes!["name"]?.Value is "pThis")
                        {
                            is_static = false;
                            break;
                        }
                    }
                    if (entrypoint is not null)
                    {
                        if (is_static)
                        {
                            code.AppendLine($"{tab}    [UnmanagedCallConv(CallConvs = [typeof(CallConv{convention})])]");
                            code.AppendLine($"{tab}    [DllImport(\"{lib}\", EntryPoint = \"{entrypoint}\")]");
                            code.Append($"{tab}    public static partial {ret_type} {method_name}(");
                            var first = true;
                            foreach (XmlNode param in member.ChildNodes)
                            {
                                if (param.Name != "param") continue;
                                if (first) first = false;
                                else code.Append($", ");
                                var param_name = param.Attributes!["name"]!.Value;
                                var param_type_node = param["type"]!;
                                var param_type = param_type_node.InnerText;
                                param_type = UpperSnakeCase().Replace(param_type, match => match.Value.ToPascalCase());
                                code.Append($"{param_type} {param_name}");
                            }
                            code.AppendLine($");");
                        }
                        else
                        {
                            code.AppendLine($"{tab}    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
                            code.Append($"{tab}    public {ret_type} {method_name}(");
                            var first = true;
                            foreach (XmlNode param in member.ChildNodes)
                            {
                                if (param.Name != "param") continue;
                                if (param.Attributes!["name"]?.Value is "pThis") continue;
                                if (first) first = false;
                                else code.Append($", ");
                                var param_name = param.Attributes!["name"]!.Value;
                                var param_type_node = param["type"]!;
                                var param_type = param_type_node.InnerText;
                                param_type = UpperSnakeCase().Replace(param_type, match => match.Value.ToPascalCase());
                                code.Append($"{param_type} {param_name}");
                            }
                            code.AppendLine($")");
                            code.AppendLine($"{tab}    {{");
                            code.Append($"{tab}        ");
                            if (ret_type != "void") code.Append($"return ");
                            code.Append($"__(");
                            first = true;
                            foreach (XmlNode param in member.ChildNodes)
                            {
                                if (param.Name != "param") continue;
                                if (first) first = false;
                                else code.Append($", ");
                                if (param.Attributes!["name"]?.Value is "pThis")
                                {
                                    code.Append($"({name}*)Unsafe.AsPointer(ref this)");
                                    continue;
                                }
                                var param_name = param.Attributes!["name"]!.Value;
                                code.Append($"{param_name}");
                            }

                            code.AppendLine($");");
                            if (ret_type == "void") code.AppendLine($"{tab}        return;");
                            code.AppendLine($"{tab}        [UnmanagedCallConv(CallConvs = [typeof(CallConv{convention})])]");
                            code.AppendLine($"{tab}        [DllImport(\"{lib}\", EntryPoint = \"{entrypoint}\")]");
                            code.Append($"{tab}        static extern {ret_type} __(");
                            first = true;
                            foreach (XmlNode param in member.ChildNodes)
                            {
                                if (param.Name != "param") continue;
                                if (first) first = false;
                                else code.Append($", ");
                                var param_name = param.Attributes!["name"]!.Value;
                                var param_type_node = param["type"]!;
                                var param_type = param_type_node.InnerText;
                                param_type = UpperSnakeCase().Replace(param_type, match => match.Value.ToPascalCase());
                                code.Append($"{param_type} {param_name}");
                            }
                            code.AppendLine($");");
                            code.AppendLine($"{tab}    }}");
                        }
                    }
                    else
                    {
                        code.AppendLine($"{tab}    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
                        code.Append($"{tab}    public {ret_type} {method_name}(");
                        var first = true;
                        foreach (XmlNode param in member.ChildNodes)
                        {
                            if (param.Name != "param") continue;
                            if (first) first = false;
                            else code.Append($", ");
                            var param_name = param.Attributes!["name"]!.Value;
                            var param_type_node = param["type"]!;
                            var param_type = param_type_node.InnerText;
                            param_type = UpperSnakeCase().Replace(param_type, match => match.Value.ToPascalCase());
                            code.Append($"{param_type} {param_name}");
                        }
                        code.AppendLine($")");
                        code.AppendLine($"{tab}    {{");
                        code.AppendLine($"{tab}        {member["code"]!.InnerText}");
                        code.AppendLine($"{tab}    }}");
                    }
                }
                else if (member.Name is "enumeration" or "struct")
                {
                    GenItem(code, member, $"{tab}    ", false);
                }
            }
            code.AppendLine($"{tab}}}");
            break;
        }
        case "enumeration":
        {
            var access = item.Attributes!["access"]!.Value;
            var raw_name = item.Attributes!["name"]!.Value;
            var name = raw_name.ToPascalCase();
            var is_flags = name.Contains("Flags");
            if (is_flags) code.AppendLine($"{tab}[Flags]");
            if (is_flags) raw_name = raw_name[..^1];
            var typ = item["type"]!.InnerText!;
            code.AppendLine($"{tab}{access} enum {name} : {typ}");
            code.AppendLine($"{tab}{{");
            foreach (XmlNode child in item.ChildNodes)
            {
                if (child.Name != "enumerator") continue;
                var child_name = child.Attributes!["name"]!.Value;
                if (child_name.StartsWith(raw_name)) child_name = child_name[raw_name.Length..];
                child_name = child_name.ToPascalCase();
                var val = child["value"]?["code"]?.InnerText;
                if (val is not null)
                {
                    val = string.Join(" | ", val.Replace(raw_name, "").Split("|").Select(a =>
                    {
                        var i = a.IndexOf(".", StringComparison.Ordinal);
                        if (i > 0)
                        {
                            var left = a[..i].ToPascalCase();
                            var right = a[(i + 1)..];
                            var left_raw = left.Contains("Flags") ? left[..^1] : left;
                            return $"{left}.{right.ToPascalCase().Replace(left_raw, "")}";
                        }
                        return a.ToPascalCase();
                    }));
                    val = $" = {val}";
                }
                else val = "";
                code.AppendLine($"{tab}    {child_name}{val},");
            }
            code.AppendLine($"{tab}}}");
            break;
        }
        case "class":
        {
            var access = item.Attributes!["access"]!.Value;
            var is_static = item.Attributes!["static"]!.Value == "true";
            var name = item.Attributes!["name"]!.Value.ToPascalCase();
            var static_mod = is_static ? "static " : "";
            code.AppendLine($"{tab}{access} {static_mod}unsafe partial class {name}");
            code.AppendLine($"{tab}{{");
            foreach (XmlNode member in item.ChildNodes)
            {
                if (member.Name == "function")
                {
                    if (!root) continue;
                    if (access != "public") continue;
                    var method_access = member.Attributes!["access"]!.Value;
                    if (method_access != "public") continue;
                    var method_name = member.Attributes!["name"]!.Value;
                    var lib = member.Attributes!["lib"]?.Value;
                    var convention = member.Attributes!["convention"]?.Value;
                    var entrypoint = member.Attributes!["entrypoint"]?.Value;
                    var ret_typ_node = member["type"];
                    var ret_type = ret_typ_node?.InnerText!;
                    ret_type = UpperSnakeCase().Replace(ret_type, match => match.Value.ToPascalCase());
                    code.AppendLine($"{tab}    [UnmanagedCallConv(CallConvs = [typeof(CallConv{convention})])]");
                    code.AppendLine($"{tab}    [DllImport(\"{lib}\", EntryPoint = \"{entrypoint}\", ExactSpelling = true)]");
                    code.Append($"{tab}    public static extern {ret_type} {method_name}(");
                    var first = true;
                    foreach (XmlNode param in member.ChildNodes)
                    {
                        if (param.Name != "param") continue;
                        if (first) first = false;
                        else code.Append($", ");
                        var param_name = param.Attributes!["name"]!.Value;
                        var param_type_node = param["type"]!;
                        var param_type = param_type_node.InnerText;
                        param_type = UpperSnakeCase().Replace(param_type, match => match.Value.ToPascalCase());
                        code.Append($"{param_type} {param_name}");
                    }
                    code.AppendLine($");");
                }
            }
            code.AppendLine($"{tab}}}");
            break;
        }
    }
}

partial class Program
{
    [GeneratedRegex(@"\b[A-Z0-9_]+\b")]
    private static partial Regex UpperSnakeCase();
}

// var code = await File.ReadAllLinesAsync(file);
// var output1 = new List<string>();
// var output2 = new List<string>();
//
// for (var i = 0; i < code.Length; i++)
// {
//     var line = code[i];
//
//     line = NativeTypeName().Replace(line, "");
//     if (string.IsNullOrWhiteSpace(line)) continue;
//     output1.Add(line);
// }
//
// for (var i = 0; i < output1.Count; i++)
// {
//     var line = output1[i];
//
//     if (line.StartsWith("        private static extern "))
//     {
//         output2.RemoveAt(output2.Count - 1);
//         continue;
//     }
//
//     line = line.Replace("static extern readonly", "static extern");
//     line = line.Replace("        public IUnknownImpl Base;", "        internal IUnknownImpl Base;");
//     if (line.StartsWith("    private partial struct")) line = line.Replace("    private partial struct", "    internal partial struct");
//     output2.Add(line);
// }
//
// var output = output2;
// await File.WriteAllLinesAsync(file, output);
//
// partial class Program
// {
//     [GeneratedRegex(@"\[[\w\s:]*NativeTypeName\(""[^""]*""\)\]")]
//     private static partial Regex NativeTypeName();
// }
