using DataLib.Extensions;

namespace DataLib.SqlServer;

public readonly struct SqlType : IEquatable<SqlType>
{
    public string SqlTypeName { get; } = string.Empty;

    internal SqlType(string sqlTypeName) =>
        this.SqlTypeName = sqlTypeName;

    public static explicit operator Type(SqlType sqlType) =>
        sqlType.ToNetType();

    public static explicit operator string(SqlType sqlType) =>
        sqlType.SqlTypeName;

    public static explicit operator SqlType(string sqlTypeName) =>
        new(sqlTypeName);

    public bool Equals(SqlType sqlType) =>
        this.GetHashCode() == sqlType.GetHashCode();

    public override int GetHashCode() =>
        this.SqlTypeName.GetHashCode(StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) =>
        obj is SqlType t && this.Equals(t);
    public static bool operator ==(SqlType left, SqlType right) =>
        left.Equals(right);

    public static bool operator !=(SqlType left, SqlType right) =>
        !(left == right);
}