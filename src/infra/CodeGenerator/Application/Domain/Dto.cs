namespace CodeGenerator.Application.Domain;

public class Dto
{
    public long Id { get; set; }
    public Guid Guid { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public long? ModuleId { get; set; }
    public string? DbObjectId { get; set; }
    public string? Comment { get; set; }
    public bool IsParamsDto { get; set; }
    public bool IsResultDto { get; set; }
    public bool IsViewModel { get; set; }
    public bool? IsList { get; set; }
    public string? BaseType { get; set; }
    public List<Property> Properties { get; set; } = [];
}
