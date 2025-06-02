namespace Library.Data.SqlServer.Dynamics.Collections;

public sealed class StoredProcedures : SqlObjects<StoredProcedure>
{
    internal StoredProcedures(IEnumerable<StoredProcedure> items)
        : base(items)
    {
    }
}
