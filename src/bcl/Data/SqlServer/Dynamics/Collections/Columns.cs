namespace Library.Data.SqlServer.Dynamics.Collections;

public sealed class Columns : SqlObjects<Column>
{
    public Columns(IEnumerable<Column> items)
        : base(items)
    {
    }
}
