using Library.Validations;

namespace Library.Data.SqlServer;

[Obsolete($"Please use `System.Data.SqlTypes.SqlDateTime`, instead.", true)]
public readonly struct SqlDateTime : IEquatable<SqlDateTime>
{
    public static readonly DateTime MaxValue = DateTime.Parse("9999/12/31 23:59:59");
    public static readonly DateTime MinValue = DateTime.Parse("1735/01/01 00:00:00");
    private readonly DateTime _value;

    public SqlDateTime(DateTime dateTime)
    {
        Check.MustBe(DateTimeHelper.IsBetween(dateTime, MinValue, MaxValue));
        this._value = dateTime;
    }

    public static explicit operator DateTime(SqlDateTime me) =>
        me._value;

    public static explicit operator SqlDateTime(DateTime me) =>
        new(me);

    public static explicit operator SqlDateTime(DateOnly me) =>
        new(me.ToDateTime(TimeOnly.MinValue));

    public static explicit operator SqlDateTime(TimeOnly me) =>
        new(DateOnly.MinValue.ToDateTime(me));

    public static string FormatDate(DateTime? date) =>
        date is { } d ? d.ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value.ToString();

    public static string FormatDate(object? date) =>
        date switch
        {
            DateTime dt => FormatDate(dt),
            DateOnly d => FormatDate(d.ToDateTime(TimeOnly.MinValue)),
            TimeOnly t => FormatDate(DateOnly.MinValue.ToDateTime(t)),
            _ => throw new NotImplementedException(),
        };

    public static bool operator !=(SqlDateTime left, SqlDateTime right) =>
        !(left == right);

    public static bool operator ==(SqlDateTime left, SqlDateTime right) =>
        left.Equals(right);

    public override bool Equals(object? obj) =>
        obj is SqlDateTime other && this.Equals(other);

    public bool Equals(SqlDateTime other) =>
        other != null && other.GetHashCode() == this.GetHashCode();

    public override int GetHashCode() =>
        this._value.GetHashCode();

    public override string ToString() =>
        FormatDate(this._value);
}