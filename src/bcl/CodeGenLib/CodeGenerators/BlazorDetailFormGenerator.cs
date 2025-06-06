namespace Library.CodeGenLib.CodeGenerators;

/// <summary>
/// Simple generator that creates a Blazor component from a DTO definition.
/// </summary>
public sealed class BlazorDetailFormGenerator : ICodeGeneratorEngine<DtoDefinition, DetailFormOptions>
{
    /// <summary>
    /// Generates code for a details form.
    /// </summary>
    public IResult<string> Generate(DtoDefinition dto, DetailFormOptions options)
    {
        Check.MustBeArgumentNotNull(dto);
        Check.MustBeArgumentNotNull(options);

        var sb = new StringBuilder();

        _ = sb.AppendLine($"@page \"/{dto.Name.ToLowerInvariant()}/detail\"");
        if (!string.IsNullOrWhiteSpace(dto.Namespace))
        {
            _ = sb.AppendLine($"@using {dto.Namespace};");
        }
        _ = sb.AppendLine("@using MediatR;");
        _ = sb.AppendLine("@using Microsoft.AspNetCore.Components;");
        _ = sb.AppendLine("@using Microsoft.JSInterop;");

        _ = sb.AppendLine();
        _ = sb.AppendLine("<EditForm Model=\"model\" OnValidSubmit=\"SaveAsync\">");
        _ = sb.AppendLine("    <DataAnnotationsValidator />");
        _ = sb.AppendLine("    <ValidationSummary />");

        foreach (var field in dto.Fields)
        {
            var type = field.Type.ToLowerInvariant();
            var component = type switch
            {
                "bool" => $"<InputCheckbox @bind-Value=\"model.{field.Name}\" />",
                "int" or "long" or "float" or "double" or "decimal" =>
                    $"<InputNumber<{field.Type}> @bind-Value=\"model.{field.Name}\" />",
                "datetime" or "datetimeoffset" =>
                    $"<InputDate @bind-Value=\"model.{field.Name}\" />",
                _ => $"<InputText @bind-Value=\"model.{field.Name}\" />"
            };
            _ = sb.AppendLine("    <div>");
            _ = sb.AppendLine($"        <label>{field.Name}</label>");
            _ = sb.AppendLine($"        {component}");
            _ = sb.AppendLine("    </div>");
        }

        _ = sb.AppendLine("    <button type=\"submit\">Save</button>");
        _ = sb.AppendLine("    <button type=\"button\" @onclick=\"CancelAsync\">Cancel</button>");
        _ = sb.AppendLine("</EditForm>");

        _ = sb.AppendLine();
        _ = sb.AppendLine("@code {");
        _ = sb.AppendLine($"    [Parameter] public {dto.Name}? Input {{ get; set; }}");
        _ = sb.AppendLine($"    private {dto.Name} model = new();");
        _ = sb.AppendLine("    [Inject] private NavigationManager Nav { get; set; } = default!;");
        _ = sb.AppendLine("    [Inject] private IMediator Mediator { get; set; } = default!;");
        _ = sb.AppendLine("    protected override void OnParametersSet() => model = Input ?? new();");
        var commandName = string.IsNullOrWhiteSpace(options.SaveCommandName) ? "" : options.SaveCommandName;
        if (!string.IsNullOrWhiteSpace(commandName))
        {
            _ = sb.AppendLine($"    private async Task SaveAsync()\n    {{\n        await Mediator.Send(new {commandName}(model));\n        Nav.NavigateTo(\"/{dto.Name.ToLowerInvariant()}s\");\n    }}");
        }
        else
        {
            _ = sb.AppendLine("    private Task SaveAsync() => Task.CompletedTask; // TODO command");
        }
        _ = sb.AppendLine("    private async Task CancelAsync() => await Js.InvokeVoidAsync(\"history.back\");");
        _ = sb.AppendLine("    [Inject] private IJSRuntime Js { get; set; } = default!;");
        _ = sb.AppendLine("}");

        return Result.Success(sb.ToString())!;
    }
}