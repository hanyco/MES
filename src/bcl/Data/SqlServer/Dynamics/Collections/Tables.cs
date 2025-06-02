namespace Library.Data.SqlServer.Dynamics.Collections;

public sealed class Tables : SqlObjects<Table>
{
    internal Tables(IEnumerable<Table> items)
        : base(items)
    {
    }
}
