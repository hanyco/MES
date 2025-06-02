using Library.Data.SqlServer.Builders.Bases;
using Library.Validations;

namespace Library.Data.SqlServer;

public static partial class SqlStatementBuilder
{
    public static string Build([DisallowNull] this IUpdateStatement statement, string indent = "    ")
    {
        Check.MustBeNotNull(statement?.TableName);
        Check.MustBe(statement.Values?.Count > 0);

        var result = new StringBuilder($"UPDATE {AddBrackets(statement.TableName)} ");

        Func<object, string> format = statement.ForceFormatValues ? FormatValue : cv => cv?.ToString() ?? DBNull.Value.ToString();
        var keyValues = statement.Values.Select(kv => $"{AddBrackets(kv.Key)} = {format(kv.Value)}").Merge(", ");
        _ = AddClause($"SET {keyValues}", indent, result);

        if (!statement.WhereClause.IsNullOrEmpty())
        {
            _ = AddClause($"WHERE {statement.WhereClause}", indent, result);
        }

        if (statement.ReturnId)
        {
            _ = result.AppendLine(";").Append("SELECT SCOPE_IDENTITY();");
        }
        return result.ToString();
    }

    public static IUpdateStatement Set([DisallowNull] this IUpdateStatement statement, string column, object value)
    {
        statement.ArgumentNotNull().Values.Add(column.ArgumentNotNull(), value);
        return statement;
    }

    public static IUpdateStatement Set([DisallowNull] this IUpdateStatement statement, params (string Column, object Value)[] columnsValue)
    {
        _ = statement.ArgumentNotNull();
        _ = columnsValue.Enumerate(cv => statement.Set(cv.Column, cv.Value)).AsReadOnly();
        return statement;
    }

    public static IUpdateStatement Set([DisallowNull] this IUpdateStatement statement, IEnumerable<(string Column, object Value)> columnsValue)
    {
        _ = statement.ArgumentNotNull();
        _ = columnsValue.Enumerate(cv => statement.Set(cv.Column, cv.Value)).AsReadOnly();
        return statement;
    }

    public static IUpdateStatement Set([DisallowNull] this IUpdateStatement statement, [DisallowNull] object entity)
        => Set(statement, entity.ArgumentNotNull().GetType().GetProperties().Where(x=>x.Name != "Id").Select(p => (p.Name, p.GetValue(entity)))!);

    public static IUpdateStatement Table([DisallowNull] this IUpdateStatement statement, [DisallowNull] string tableName)
        => statement.Fluent(statement.ArgumentNotNull().TableName = tableName.ArgumentNotNull()).GetValue();

    public static IUpdateStatement Table<TTable>([DisallowNull] this IUpdateStatement statement)
    {
        var table = GetTable<TTable>();
        return Table(statement, table.Name).SetSchema(table.Schema);
    }

    public static IUpdateStatement Update() =>
        new UpdateStatement();

    public static IUpdateStatement Update([DisallowNull] string tableName)
        => new UpdateStatement { TableName = tableName.ArgumentNotNull() };

    public static IUpdateStatement Where([DisallowNull] this IUpdateStatement statement, string? whereClause)
        => statement.Fluent(statement.ArgumentNotNull().WhereClause = whereClause).GetValue();

    private class UpdateStatement : IUpdateStatement
    {
        public bool ForceFormatValues { get; set; } = true;
        public bool ReturnId { get; set; }
        public string? Schema { get; set; }
        public string TableName { get; set; } = null!;
        public IDictionary<string, object> Values { get; } = new Dictionary<string, object>();
        public string? WhereClause { get; set; }
    }
}