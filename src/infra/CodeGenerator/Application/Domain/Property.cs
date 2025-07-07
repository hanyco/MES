using CodeGenerator.Application.Services;

namespace CodeGenerator.Application.Domain;

public class Property
{
    public long Id { get; set; }
    public long ParentEntityId { get; set; }
    public int PropertyType { get; set; }
    public string? TypeFullName { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool? HasSetter { get; set; }
    public bool? HasGetter { get; set; }
    public bool? IsList { get; set; }
    public bool? IsNullable { get; set; }
    public string? Comment { get; set; }
    public string? DbObjectId { get; set; }
    public Guid Guid { get; set; }
    public long? DtoId { get; set; }
}
