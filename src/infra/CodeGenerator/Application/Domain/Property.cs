namespace CodeGenerator.Application.Domain;

public class Property
{
    public string? Comment { get; set; }
    public string? DbObjectId { get; set; }
    public bool? HasGetter { get; set; }
    public bool? HasSetter { get; set; }
    public long Id { get; set; }
    public bool? IsList { get; set; }
    public bool? IsNullable { get; set; }
    public string Name { get; set; } = string.Empty;
    public long ParentEntityId { get; set; }
    public int PropertyType { get; set; }
    public string? TypeFullName { get; set; }
}