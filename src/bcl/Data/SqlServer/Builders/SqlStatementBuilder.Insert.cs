using Library.Data.SqlServer.Builders.Bases;
using Library.Validations;

namespace Library.Data.SqlServer;

public partial class SqlStatementBuilder
{
    public static string Build([DisallowNull] this IInsertStatement statement, string indent = "    ")
    {
        Check.MustBeNotNull(statement?.TableName);
        Check.MustHaveAny(statement.Values);

        var result = new StringBuilder($"INSERT INTO {AddBrackets(statement.TableName)}");
        _ = AddClause($"({statement.Values.Select(x => AddBrackets(x.Key.NotNull())).Merge(", ")})", indent, result);
        string clause;
        if (!statement.ForceFormatValues)
        {
            clause = $"VALUES ({statement.Values.Select(x => x.Value.NotNull().ToString()!).Merge(", ")})";
        }
        else
        {
            clause = $"VALUES ({statement.Values.Select(x => FormatValue(x.Value.NotNull())).Merge(", ")})";
        }
        _ = AddClause(clause, indent, result);

        if (statement.ReturnId)
        {
            _ = result.AppendLine(";").Append("SELECT SCOPE_IDENTITY();");
        }

        return result.ToString();
    }

    public static IInsertStatement Insert() =>
        new InsertStatement();

    public static IInsertStatement Into([DisallowNull] this IInsertStatement statement, string tableName) =>
        statement.With(x => x.TableName = tableName);

    public static IInsertStatement Into<TTable>([DisallowNull] this IInsertStatement statement)
    {
        var table = GetTable<TTable>();
        return Into(statement, table.Name).SetSchema(table.Schema);
    }

    public static IInsertStatement Values([DisallowNull] this IInsertStatement statement, object model)
    {
        Check.MustBeArgumentNotNull(statement);
        Check.MustBeArgumentNotNull(model);

        IEnumerable<(string, object?)> values = model.GetType().GetProperties().Select(x => (x.Name, x.GetValue(model)));//.Compact(isNotNull)!;
        return Values(statement, values.Except(x => x.Item1.Equals("Id", StringComparison.OrdinalIgnoreCase)));
        //static bool isNotNull((string? columnName, object? value) x) => x.columnName.IsNullOrEmpty() || x.value is null;
    }

    public static IInsertStatement Values([DisallowNull] this IInsertStatement statement, params (string ColumnName, object? Value)[] values) =>
        statement.With(x => x.Values(values.Select(x => new KeyValuePair<string, object?>(x.ColumnName, x.Value))));

    public static IInsertStatement Values([DisallowNull] this IInsertStatement statement, IEnumerable<(string ColumnName, object? Value)> values) =>
        statement.Values(values.ToArray());

    public static IInsertStatement Values([DisallowNull] this IInsertStatement statement, IEnumerable<KeyValuePair<string, object?>> values) =>
        statement.Values(values.ToArray());

    public static IInsertStatement Values([DisallowNull] this IInsertStatement statement, params KeyValuePair<string, object?>[] values) =>
        statement.With(x => x.Values.AddRange(values));

    private struct InsertStatement : IInsertStatement
    {
        public InsertStatement()
        {
        }

        public bool ForceFormatValues { get; set; } = true;
        public bool ReturnId { get; set; }
        public string? Schema { get; set; }
        public string TableName { get; set; } = null!;
        public IDictionary<string, object> Values { get; } = new Dictionary<string, object>();
    }
}