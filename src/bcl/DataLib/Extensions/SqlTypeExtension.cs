using System.Diagnostics.CodeAnalysis;

using DataLib.Extensions;
using DataLib.SqlServer;

using Library.Coding;
using Library.Extensions;
using Library.Validations;
namespace DataLib.Extensions;

public static class SqlTypeExtension
{
    private static readonly Dictionary<SqlType, Type> _data = new()
    {
        [new("smallint")] = typeof(short),
        [new("int")] = typeof(int),
        [new("bigint")] = typeof(long),
        [new("datetime")] = typeof(DateTime),
        [new("datetime2")] = typeof(DateTime),
        [new("varchar")] = typeof(string),
        [new("nvarchar")] = typeof(string),
        [new("bit")] = typeof(bool),
        [new("date")] = typeof(DateOnly)
    };

    extension(SqlType)
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

        public static IEnumerable<(SqlType SqlType, Type NetType)> GetSqlTypes() =>
            _data.Select(x => (x.Key, x.Value));

        public static bool IsSqlType(string typeName) =>
            !typeName.IsNullOrEmpty() && _data.Any(x => x.Key.SqlTypeName.Equals(typeName, StringComparison.OrdinalIgnoreCase));

        public static Type ToNetType([DisallowNull] string sqlTypeName)
        {
            Check.MustBeArgumentNotNull(sqlTypeName);
            return _data
                .FirstOrDefault(x => x.Key.SqlTypeName.Equals(sqlTypeName, StringComparison.OrdinalIgnoreCase))
                .EnsureNotNull(() => ".NET type not found.").Value;
        }

        public static SqlType ToSqlType([DisallowNull] string sqlTypeName)
        {
            Check.MustBeArgumentNotNull(sqlTypeName);
            return _data
                .FirstOrDefault(x => x.Key.SqlTypeName.Equals(sqlTypeName, StringComparison.OrdinalIgnoreCase))
                .EnsureNotNull(() => ".NET type not found.").Key;
        }
    }

    extension(SqlType @this)
    {
        public Type ToNetType() =>
            ToNetType(@this.SqlTypeName);

        public int GetTypeCode()
        {
            Check.MustBeArgumentNotNull(@this);
            return TypePath.GetTypeCode(ToNetType(@this));
        }
    }
}
