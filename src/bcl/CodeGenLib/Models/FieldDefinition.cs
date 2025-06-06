namespace Library.CodeGenLib.Models;

/// <summary>
/// Represents a single property in a DTO for code generation.
/// </summary>
public class FieldDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Comment { get; set; }
}
