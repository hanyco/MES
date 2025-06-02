namespace Library.Data.SqlServer.Dynamics;

public sealed class StoredProcedureParam(StoredProcedure owner, string name, string connectionString) : SqlObject<StoredProcedureParam, StoredProcedure>(owner, name, connectionString: connectionString)
{
    public string? DefaultValue { get; set; }
    public long Id { get; set; }
    public int Length { get; set; }
    public int NumericPrecision { get; set; }
    public string? SqlDataType { get; set; }
}
