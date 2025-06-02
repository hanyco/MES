namespace Library.Data.SqlServer;

public partial class SqlStatementBuilder
{
    public static string CreateSelect([DisallowNull] string tableName, params string[] columns)
        => Select().Columns(columns).From(tableName).Build();
}
