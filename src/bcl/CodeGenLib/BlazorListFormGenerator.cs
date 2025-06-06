using System.Text;
using System.Linq;

namespace Library.CodeGenLib;

/// <summary>
/// Simple generator that creates a Blazor component from a DTO definition.
/// </summary>
public sealed class BlazorListFormGenerator : ICodeGeneratorEngine<DtoDefinition, ListFormOptions>
{
    /// <summary>
    /// Generates code for listing DTOs.
    /// </summary>
    public IResult<string> Generate(DtoDefinition dto, ListFormOptions options)
    {
        Check.MustBeArgumentNotNull(dto);
        Check.MustBeArgumentNotNull(options);

        var sb = new StringBuilder();

        _ = sb.AppendLine($"@page \"/{dto.Name.ToLowerInvariant()}s\"");
        if (!string.IsNullOrWhiteSpace(dto.Namespace))
        {
            _ = sb.AppendLine($"@using {dto.Namespace};");
        }
        _ = sb.AppendLine("@using System.Linq;");

        _ = sb.AppendLine();
        _ = sb.AppendLine($"<h3>{dto.Name} List</h3>");

        if (options.IncludeCreateButton)
        {
            _ = sb.AppendLine("<button @onclick=\"CreateAsync\">ایجاد</button>");
        }
        if (options.IncludeDeleteButton)
        {
            if (options.EnableMultiSelect)
            {
                _ = sb.AppendLine("<button @onclick=\"DeleteAsync\" disabled=\"@(Selected.Count == 0)\">حذف</button>");
            }
            else
            {
                _ = sb.AppendLine("<button @onclick=\"DeleteAsync\">حذف</button>");
            }
        }

        _ = sb.AppendLine("<table class=\"table\">");
        _ = sb.AppendLine("    <thead>");
        _ = sb.Append("        <tr>");
        if (options.EnableMultiSelect)
        {
            _ = sb.Append("<th></th>");
        }
        foreach (var field in dto.Fields)
        {
            _ = sb.Append($"<th>{field.Name}</th>");
        }
        _ = sb.AppendLine("</tr>");
        _ = sb.AppendLine("    </thead>");
        _ = sb.AppendLine("    <tbody>");
        _ = sb.AppendLine("@foreach (var item in items)");
        _ = sb.AppendLine("{");
        _ = sb.AppendLine("    <tr>");
        if (options.EnableMultiSelect)
        {
            _ = sb.AppendLine("        <td><input type=\"checkbox\" @bind=\"item.IsSelected\" /></td>");
        }
        foreach (var field in dto.Fields)
        {
            _ = sb.AppendLine($"        <td>@item.{field.Name}</td>");
        }
        _ = sb.AppendLine("    </tr>");
        _ = sb.AppendLine("}");
        _ = sb.AppendLine("    </tbody>");
        _ = sb.AppendLine("</table>");

        _ = sb.AppendLine();
        _ = sb.AppendLine("@code {");
        _ = sb.AppendLine($"    private List<{dto.Name}> items = new();");
        if (options.EnableMultiSelect)
        {
            _ = sb.AppendLine($"    private List<{dto.Name}> Selected => items.Where(x => x.IsSelected).ToList();");
        }
        _ = sb.AppendLine("    private Task CreateAsync() => Task.CompletedTask; // TODO");
        if (options.IncludeDeleteButton)
        {
            _ = sb.AppendLine("    private Task DeleteAsync() => Task.CompletedTask; // TODO");
        }
        _ = sb.AppendLine("}");

        return Result.Success(sb.ToString())!;
    }
}