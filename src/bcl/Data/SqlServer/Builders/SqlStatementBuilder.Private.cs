namespace Library.Data.SqlServer;

public static partial class SqlStatementBuilder
{
    public static string AddBrackets(in string entity)
    {
        var entities = entity.Split('.').Compact();
        var result = entities.Select(e =>
        {
            var r = e.StartsWith('[') ? e : $"[{e}";
            r = r.EndsWith(']') ? r : $"{r}]";
            return r;
        }).Merge('.');
        return result;
    }

    private static StringBuilder AddClause(in string clause, in string indent, in StringBuilder result) =>
        result.AppendLine().Append($"{indent}{clause}");

    private static string FormatValue(object? value)
    {
        var stringValue = value?.ToString();
        return stringValue?.StartsWith('\'') ?? false
            ? stringValue
            : value switch
            {
                int i => i.ToString(),
                float f => f.ToString(),
                decimal d => d.ToString(),
                long l => l.ToString(),

                "null" or "NULL" => "NULL",
                string and { Length: 0 } => "''",
                string s => $"N'{s.Trim('\'')}'",

                null => DBNull.Value.ToString(),
                _ => $"'{stringValue?.Trim('\'')}'",
            };
    }
}