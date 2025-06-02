namespace Library.Data.SqlServer.Dynamics;

public sealed class ForeignKeyInfo
{
    public string? ReferencedColumn { get; set; }
    public string? ReferencedTable { get; set; }
}
