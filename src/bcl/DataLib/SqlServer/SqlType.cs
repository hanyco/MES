using Library.Coding;
﻿namespace DataLib.SqlServer;

public static class SqlTypeUtils
{
    public static string FormatDate(DateTime date, in string onNull = "NULL", bool isForInterpolation = false) =>
        DBNull.Value.Equals(date)
            ? onNull
            : isForInterpolation ? $"{date:yyyy-MM-dd HH:mm:ss}" : $"'{date:yyyy-MM-dd HH:mm:ss}'";

    public static string FormatDate(DateTime? date, in string onNull = "NULL", bool isForInterpolation = false) =>
        date == null || DBNull.Value.Equals(date) ? onNull : FormatDate(date.Value, onNull, isForInterpolation);

    public static string FormatDate(object? data, in string onNull = "null", bool isForInterpolation = false) =>
        data switch
        {
            DateTime dt => FormatDate(dt, onNull, isForInterpolation),
            DateOnly d => FormatDate(d.ToDateTime(TimeOnly.MinValue), onNull, isForInterpolation),
            TimeOnly t => FormatDate(DateOnly.MinValue.ToDateTime(t), onNull, isForInterpolation),
            null => FormatDate((DateTime?)data, onNull, isForInterpolation),
            _ => throw new NotImplementedException(),
        };

    public static IEnumerable<(string SqlTypeName, Type Type)> GetSqlTypes()
    {
        yield return ("smallint", typeof(short)); // Map "smallint" to short
        yield return ("int", typeof(int)); // Map "int" to int
        yield return ("bigint", typeof(long)); // Map "bigint" to long.
        yield return ("datetime", typeof(DateTime)); // Map "datetime" or "datetime2" to DateTime.
        yield return ("datetime2", typeof(DateTime)); // Map "datetime" or "datetime2" to DateTime.
        yield return ("varchar", typeof(string)); // Map "varchar" or "nvarchar" to string.
        yield return ("nvarchar", typeof(string)); // Map "varchar" or "nvarchar" to string.
        yield return ("bit", typeof(bool)); // Map "bit" to bool.
        yield return ("date", typeof(DateOnly)); // Map "date" to DateOnly.
    }

    public static TypePath ToNetTypeName(string sqlTypeName)
    {
        foreach (var (name, type) in GetSqlTypes())
        {
            if (string.Equals(name, sqlTypeName, StringComparison.OrdinalIgnoreCase))
            {
                return TypePath.Parse(type.FullName ?? type.Name);
            }
        }

        throw new KeyNotFoundException();
    }
}
