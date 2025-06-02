namespace Library.Data.SqlServer.Dynamics.Collections;

public sealed class StoredProcedureParams : SqlObjects<StoredProcedureParam>
{
    internal StoredProcedureParams(IEnumerable<StoredProcedureParam> items)
        : base(items)
    {
    }
}
