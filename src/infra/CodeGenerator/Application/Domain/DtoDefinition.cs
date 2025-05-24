using System.Reflection.Metadata;

namespace CodeGenerator.Application.Domain;

/// <summary>
/// Describes a DTO model for code generation.
/// </summary>
public class DtoDefinition
{
    public string? Comment { get; set; }
    public List<FieldDefinition> Fields { get; set; } = [];
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
}