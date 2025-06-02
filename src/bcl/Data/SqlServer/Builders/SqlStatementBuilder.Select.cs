using Library.Data.SqlServer;
using Library.Data.SqlServer.Builders.Bases;
using Library.Validations;

namespace Library.Data.SqlServer;

public static partial class SqlStatementBuilder
{
    public static ISelectStatement AddColumn([DisallowNull] this ISelectStatement statement, params string[] columns)
        => statement.ArgumentNotNull().With(x => columns.Compact().Enumerate(c => x.Columns.Add(c)).AsReadOnly());

    public static ISelectStatement AddColumns([DisallowNull] this ISelectStatement statement, IEnumerable<string> columns)
        => statement.AddColumn(columns.ToArray());

    public static ISelectStatement AllColumns([DisallowNull] this ISelectStatement statement)
        => statement.Star();

    public static ISelectStatement Ascending([DisallowNull] this ISelectStatement statement)
        => statement.ArgumentNotNull().With(x => x.OrderByDirection = OrderByDirection.Ascending);

    public static string Build([DisallowNull] this ISelectStatement statement, string indent = "    ")
    {
        Check.MustBeArgumentNotNull(statement?.TableName);
        var result = new StringBuilder("SELECT");
        if (statement.TopCount > 0)
        {
            _ = result.Append($" TOP ({statement.TopCount})");
        }
        if (statement.Columns.Any())
        {
            foreach (var column in statement.Columns)
            {
                _ = result.Append($" {AddBrackets(column)},");
            }
            _ = result.Remove(result.Length - 1, 1);
        }
        else
        {
            _ = result.Append(" *");
        }
        _ = AddClause($"FROM {AddBrackets($"{statement.Schema}.{statement.TableName}")}", indent, result);
        if (statement.WithNoLock)
        {
            _ = AddClause("WITH (NOLOCK)", indent, result);
        }
        if (!statement.WhereClause.IsNullOrEmpty())
        {
            _ = AddClause($"WHERE {statement.WhereClause}", indent, result);
        }
        if (statement.OrderByDirection != OrderByDirection.None)
        {
            Check.MustBeArgumentNotNull(statement.OrderByColumn);
            _ = AddClause($"ORDER BY {AddBrackets(statement.OrderByColumn)}", indent, result);
            _ = result.Append(statement.OrderByDirection switch
            {
                OrderByDirection.Ascending => " ASC",
                OrderByDirection.Descending => " DESC",
                _ => throw new NotImplementedException(),
            });
        }
        return result.ToString();
    }

    public static ISelectStatement ClearOrdering([DisallowNull] this ISelectStatement statement)
        => statement.ArgumentNotNull().Fluent(() =>
        {
            statement.OrderByColumn = null;
            statement.OrderByDirection = OrderByDirection.None;
        }).GetValue();

    public static ISelectStatement Columns([DisallowNull] this ISelectStatement statement, params string[] columns)
        => statement.ArgumentNotNull().With(x => x.Columns.ClearAndAddRange(columns.Compact()));

    public static ISelectStatement Columns([DisallowNull] this ISelectStatement statement, IEnumerable<string> columns)
    {
        Check.MustBeArgumentNotNull(statement, nameof(statement));
        statement.Columns.Clear();
        return statement.AddColumns(columns);
    }

    public static ISelectStatement Descending([DisallowNull] this ISelectStatement statement)
        => statement.ArgumentNotNull().With(x => x.OrderByDirection = OrderByDirection.Descending);

    public static ISelectStatement From([DisallowNull] this ISelectStatement statement, [DisallowNull] string tableName)
        => statement.ArgumentNotNull().With(x => x.TableName = tableName.ArgumentNotNull());

    public static ISelectStatement OrderBy([DisallowNull] this ISelectStatement statement, string? column)
        => statement.ArgumentNotNull().With(x => x.OrderByColumn = column);

    public static ISelectStatement OrderByDescending([DisallowNull] this ISelectStatement statement, string? column)
        => OrderBy(statement, column).Descending();

    public static ISelectStatement Select<TTable>() =>
        Select(typeof(TTable));

    public static ISelectStatement Select([DisallowNull] string tableName)
        => new SelectStatement { TableName = tableName.ArgumentNotNull() };

    public static ISelectStatement Select()
        => new SelectStatement();

    public static ISelectStatement Select(Type type)
    {
        var table = GetTable(type);
        var result = Select(table.Name)
            .SetSchema(table.Schema)
            .AddColumns(table.Columns.Select(x => x.Name));
        return result;
    }

    public static TStatementOnTable SetSchema<TStatementOnTable>([DisallowNull] this TStatementOnTable statement, string? schema) where TStatementOnTable : IStatementOnTable =>
        statement.ArgumentNotNull().With(x => x.Schema = schema);

    public static ISelectStatement Top([DisallowNull] this ISelectStatement statement, int? topCount) =>
        statement.ArgumentNotNull().With(x => x.TopCount = topCount);

    public static ISelectStatement Star([DisallowNull] this ISelectStatement statement) =>
        statement.ArgumentNotNull().Fluent(statement.Columns.Clear).GetValue();

    public static ISelectStatement Where([DisallowNull] this ISelectStatement statement, string? whereClause) =>
        statement.ArgumentNotNull().With(x => x.WhereClause = whereClause);

    public static ISelectStatement WithNoLock(this ISelectStatement statement, bool withNoLock = true) =>
        statement.With(x => x.WithNoLock = withNoLock);

    private struct SelectStatement : ISelectStatement
    {
        public SelectStatement()
        {
            this.TableName = string.Empty;
            this.WhereClause = this.OrderByColumn = null;
        }

        public IList<string> Columns { get; } = [];
        public string? OrderByColumn { get; set; }
        public OrderByDirection OrderByDirection { get; set; } = OrderByDirection.None;
        public string? Schema { get; set; }
        public string TableName { get; set; }
        public int? TopCount { get; set; }
        public string? WhereClause { get; set; }
        public bool WithNoLock { get; set; }
    }
}