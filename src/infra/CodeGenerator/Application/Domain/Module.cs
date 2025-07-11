namespace CodeGenerator.Application.Domain;

public class Module
{
    public Guid Guid { get; set; }
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long ParentId { get; set; }
}