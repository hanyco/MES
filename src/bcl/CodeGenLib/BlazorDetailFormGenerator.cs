namespace Library.CodeGenLib;

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
}