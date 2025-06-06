using System.Text;
using Library.CodeGenLib.Models;
using Library.CodeGenLib.Back;
using Library.Resulting;
using Library.Validations;

namespace Library.CodeGenLib;

/// <summary>
/// Simple generator that creates a Blazor component from a DTO definition.
/// </summary>
public sealed class BlazorCodeGenerator : ICodeGeneratorEngine
{
    /// <summary>
    /// Default detail form generation when no specific options are provided.
    /// </summary>
    public IResult<string> Generate(DtoDefinition dto) =>
        GenerateDetail(dto, new());

    /// <summary>
    /// Generates code for listing DTOs.
    /// </summary>
    public IResult<string> GenerateList(DtoDefinition dto, ListFormOptions options)
    {
        Check.MustBeArgumentNotNull(dto);
        Check.MustBeArgumentNotNull(options);

        var sb = new StringBuilder();

        sb.AppendLine($"@page \"/{dto.Name.ToLowerInvariant()}s\"");
        if (!string.IsNullOrWhiteSpace(dto.Namespace))
        {
            sb.AppendLine($"@using {dto.Namespace};");
        }

        sb.AppendLine();
        sb.AppendLine($"<h3>{dto.Name} List</h3>");

        if (options.IncludeCreateButton)
        {
            sb.AppendLine("<button @onclick=\"CreateAsync\">ایجاد</button>");
        }
        if (options.IncludeDeleteButton)
        {
            sb.AppendLine("<button @onclick=\"DeleteAsync\" disabled=\"@(Selected.Count == 0)\">حذف</button>");
        }

        sb.AppendLine("<table class=\"table\">");
        sb.AppendLine("    <thead>");
        sb.Append("        <tr>");
        if (options.EnableMultiSelect)
        {
            sb.Append("<th></th>");
        }
        foreach (var field in dto.Fields)
        {
            sb.Append($"<th>{field.Name}</th>");
        }
        sb.AppendLine("</tr>");
        sb.AppendLine("    </thead>");
        sb.AppendLine("    <tbody>");
        sb.AppendLine("@foreach (var item in items)");
        sb.AppendLine("{");
        sb.AppendLine("    <tr>");
        if (options.EnableMultiSelect)
        {
            sb.AppendLine("        <td><input type=\"checkbox\" @bind=\"item.IsSelected\" /></td>");
        }
        foreach (var field in dto.Fields)
        {
            sb.AppendLine($"        <td>@item.{field.Name}</td>");
        }
        sb.AppendLine("    </tr>");
        sb.AppendLine("}");
        sb.AppendLine("    </tbody>");
        sb.AppendLine("</table>");

        sb.AppendLine();
        sb.AppendLine("@code {");
        sb.AppendLine($"    private List<{dto.Name}> items = new();");
        if (options.EnableMultiSelect)
        {
            sb.AppendLine($"    private List<{dto.Name}> Selected => items.Where(x => x.IsSelected).ToList();");
        }
        sb.AppendLine("    private Task CreateAsync() => Task.CompletedTask; // TODO");
        if (options.IncludeDeleteButton)
        {
            sb.AppendLine("    private Task DeleteAsync() => Task.CompletedTask; // TODO");
        }
        sb.AppendLine("}");

        return Result.Success(sb.ToString())!;
    }

    /// <summary>
    /// Generates code for a details form.
    /// </summary>
    public IResult<string> GenerateDetail(DtoDefinition dto, DetailFormOptions options)
    {
        Check.MustBeArgumentNotNull(dto);
        Check.MustBeArgumentNotNull(options);

        var sb = new StringBuilder();

        sb.AppendLine($"@page \"/{dto.Name.ToLowerInvariant()}/detail\"");
        if (!string.IsNullOrWhiteSpace(dto.Namespace))
        {
            sb.AppendLine($"@using {dto.Namespace};");
        }

        sb.AppendLine();
        sb.AppendLine("<EditForm Model=\"model\" OnValidSubmit=\"SaveAsync\">");
        sb.AppendLine("    <DataAnnotationsValidator />");
        sb.AppendLine("    <ValidationSummary />");

        foreach (var field in dto.Fields)
        {
            var component = field.Type.ToLowerInvariant() switch
            {
                "bool" => $"<InputCheckbox @bind-Value=\"model.{field.Name}\" />",
                _ => $"<InputText @bind-Value=\"model.{field.Name}\" />"
            };
            sb.AppendLine("    <div>");
            sb.AppendLine($"        <label>{field.Name}</label>");
            sb.AppendLine($"        {component}");
            sb.AppendLine("    </div>");
        }

        sb.AppendLine("    <button type=\"submit\">Save</button>");
        sb.AppendLine("    <button type=\"button\" @onclick=\"CancelAsync\">Cancel</button>");
        sb.AppendLine("</EditForm>");

        sb.AppendLine();
        sb.AppendLine("@code {");
        sb.AppendLine($"    [Parameter] public {dto.Name}? Input {{ get; set; }}");
        sb.AppendLine($"    private {dto.Name} model = new();");
        sb.AppendLine("    [Inject] private NavigationManager Nav { get; set; } = default!;");
        sb.AppendLine("    [Inject] private IMediator Mediator { get; set; } = default!;");
        sb.AppendLine("    protected override void OnParametersSet() => model = Input ?? new();");
        var commandName = string.IsNullOrWhiteSpace(options.SaveCommandName) ? "" : options.SaveCommandName;
        if (!string.IsNullOrWhiteSpace(commandName))
        {
            sb.AppendLine($"    private async Task SaveAsync() => await Mediator.Send(new {commandName}(model));");
        }
        else
        {
            sb.AppendLine("    private Task SaveAsync() => Task.CompletedTask; // TODO command");
        }
        sb.AppendLine("    private async Task CancelAsync() => await Js.InvokeVoidAsync(\"history.back\");");
        sb.AppendLine("    [Inject] private IJSRuntime Js { get; set; } = default!;");
        sb.AppendLine("}");

        return Result.Success(sb.ToString())!;
    }

    /// <summary>
    /// Generates Blazor code from the first class found in the namespace.
    /// </summary>
    public IResult<string> Generate(INamespace nameSpace)
    {
        Check.MustBeArgumentNotNull(nameSpace);
        var classType = nameSpace.Types.OfType<IClass>().FirstOrDefault();
        if (classType == null)
        {
            return Result.Fail<string>("No class definition found.");
        }

        var dto = new DtoDefinition { Name = classType.Name, Namespace = nameSpace.Name };
        foreach (var prop in classType.Members.OfType<IProperty>())
        {
            dto.Fields.Add(new FieldDefinition { Name = prop.Name, Type = prop.Type.FullName });
        }

        return GenerateDetail(dto, new());
    }
}
