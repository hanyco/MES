namespace Library.CodeGenLib.Models;

/// <summary>
/// Defines a DTO used during code generation.
/// </summary>
public class DtoDefinition
{
    public string Namespace { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public List<FieldDefinition> Fields { get; } = [];
}
