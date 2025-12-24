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

code.AppendLine($"namespace Vma;");

var name_map = new Dictionary<string, string>();

foreach (XmlNode item in root.ChildNodes)
{
    CollectName(name_map, item);
}
foreach (XmlNode item in root.ChildNodes)
{
    GenItem(name_map, code, item);
}

File.WriteAllText(output_path, code.ToString());

return;

static void CollectName(Dictionary<string, string> name_map, XmlNode item)
{
    switch (item.Name)
    {
        case "struct":
        {
            var raw_name = item.Attributes!["name"]!.Value;
            var name = raw_name;
            if (name.StartsWith("Vma")) name = name[3..];
            if (name.EndsWith("_T")) name = name[..^2];
            name_map[raw_name] = name;
            break;
        }
        case "enumeration":
        {
            var raw_name = item.Attributes!["name"]!.Value;
            var name = raw_name;
            if (name.StartsWith("Vma")) name = name[3..];
            var is_flags = name.EndsWith("FlagBits");
            if (is_flags) name = name.Replace("FlagBits", "Flags");
            name_map[raw_name] = name;
            var prefix = raw_name;
            if (is_flags) prefix = prefix[..^8];
            foreach (XmlNode child in item.ChildNodes)
            {
                if (child.Name != "enumerator") continue;
                var child_raw_name = child.Attributes!["name"]!.Value;
                if (child_raw_name.EndsWith("_MAX_ENUM")) continue;
                var child_name = child_raw_name.ToPascalCase();
                if (child_name.StartsWith(prefix)) child_name = child_name[prefix.Length..];
                name_map[child_raw_name] = child_name;
            }
            break;
        }
    }
}
static void GenItem(Dictionary<string, string> name_map, StringBuilder code, XmlNode item)
{
    switch (item.Name)
    {
        case "struct":
        {
            var raw_name = item.Attributes!["name"]!.Value;
            var name = name_map.GetValueOrDefault(raw_name, raw_name);
            code.AppendLine($"public unsafe struct {name}");
            code.AppendLine($"{{");
            foreach (XmlNode member in item.ChildNodes)
            {
                if (member.Name == "field")
                {
                    var field_name = member.Attributes!["name"]!.Value;
                    field_name = field_name.ToPascalCase();
                    if (field_name == name) field_name = member.Attributes!["name"]!.Value;
                    var typ_node = member["type"]!;
                    var typ = typ_node.InnerText;
                    typ = (name, field_name) switch
                    {
                        ("AllocatorCreateInfo", "Flags") => "AllocatorCreateFlags",
                        ("AllocatorCreateInfo", "PTypeExternalMemoryHandleTypes") => "ExternalMemoryHandleTypeFlags*",
                        ("AllocationCreateInfo", "Flags") => "AllocationCreateFlags",
                        ("AllocationCreateInfo", "RequiredFlags") => "MemoryPropertyFlags",
                        ("AllocationCreateInfo", "PreferredFlags") => "MemoryPropertyFlags",
                        ("PoolCreateInfo", "Flags") => "PoolCreateFlags",
                        ("DefragmentationInfo", "Flags") => "DefragmentationFlags",
                        ("VirtualBlockCreateInfo", "Flags") => "VirtualBlockCreateFlags",
                        ("VirtualAllocationCreateInfo", "Flags") => "VirtualAllocationCreateFlags",
                        _ => name_map.GetValueOrDefault(typ, typ),
                    };

                    var count = typ_node.Attributes["count"]?.Value;
                    if (count is not null) typ = $"InlineArray{count}<{typ}>";
                    code.AppendLine($"    public {typ} {field_name};");
                }
            }
            code.AppendLine($"}}");
            break;
        }
        case "enumeration":
        {
            var raw_name = item.Attributes!["name"]!.Value;
            var name = name_map.GetValueOrDefault(raw_name, "ERROR");
            var is_flags = name.EndsWith("Flags");
            var typ = item["type"]!.InnerText!;
            if (is_flags)
            {
                code.AppendLine($"[Flags]");
                code.AppendLine($"public enum {name} : {typ}");
            }
            else
            {
                code.AppendLine($"public enum {name} : {typ}");
            }
            code.AppendLine($"{{");
            foreach (XmlNode child in item.ChildNodes)
            {
                if (child.Name != "enumerator") continue;
                var child_name = child.Attributes!["name"]!.Value;
                if (child_name.EndsWith("_MAX_ENUM")) continue;
                child_name = name_map.GetValueOrDefault(child_name, child_name);
                var val = child["value"]?["code"]?.InnerText;
                if (val is not null)
                {
                    val = string.Join(" | ", val.Split("|").Select(a =>
                    {
                        a = a.Trim();
                        var i = a.IndexOf('.');
                        if (i > 0)
                        {
                            var left = a[..i].ToPascalCase();
                            var right = a[(i + 1)..];
                            left = name_map.GetValueOrDefault(left, left);
                            right = name_map.GetValueOrDefault(right, right);
                            return $"{left}.{right}";
                        }
                        return name_map.GetValueOrDefault(a, a);
                    }));
                    val = $" = {val}";
                }
                else val = "";
                code.AppendLine($"    {child_name}{val},");
            }
            code.AppendLine($"}}");
            break;
        }
        case "class":
        {
            var name = item.Attributes!["name"]!.Value.ToPascalCase();
            code.AppendLine($"public static unsafe partial class {name}");
            code.AppendLine($"{{");
            foreach (XmlNode member in item.ChildNodes)
            {
                if (member.Name == "function")
                {
                    var method_raw_name = member.Attributes!["name"]!.Value;
                    var method_name = method_raw_name;
                    if (method_name.StartsWith("vma")) method_name = method_name[3..];
                    var lib = member.Attributes!["lib"]?.Value;
                    var convention = member.Attributes!["convention"]?.Value;
                    var ret_typ_node = member["type"];
                    var ret_type = ret_typ_node?.InnerText!;
                    ret_type = UpperSnakeCase().Replace(ret_type, match => match.Value.ToPascalCase());
                    code.AppendLine($"    [UnmanagedCallConv(CallConvs = [typeof(CallConv{convention})])]");
                    code.AppendLine($"    [DllImport(\"{lib}\", EntryPoint = \"{method_raw_name}\", ExactSpelling = true)]");
                    code.Append($"    public static extern {ret_type} {method_name}(");
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
            code.AppendLine($"}}");
            break;
        }
    }
}

partial class Program
{
    [GeneratedRegex(@"\b[A-Z0-9_]+\b")]
    private static partial Regex UpperSnakeCase();
}
