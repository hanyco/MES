namespace Library.Data.SqlServer.Dynamics;

public sealed class Column : SqlObject<Column, Table>, IEquatable<Column>
{
    public Column(Table owner, string name, string connectionString)
        : base(owner, name, connectionString: connectionString)
    {
    }

    public string CollationName { get; set; }
    public string DataType { get; set; }
    public ForeignKeyInfo ForeignKeyInfo { get; set; }
    public bool IsForeignKey { get; set; }
    public long UniqueId { get; set; }
    public bool IsIdentity { get; set; }
    public bool IsNullable { get; set; }
    public int MaxLength { get; set; }
    public int Position { get; set; }
    public int Precision { get; set; }

    public bool Equals(Column? other)
    {
        if (other is null)
        {
            return false;
        }

        return ReferenceEquals(this, other) || string.Equals(this.Name, other.Name, StringComparison.Ordinal);
    }

    public static bool operator ==(Column left, Column right) => Equals(left, right);
    public static bool operator !=(Column left, Column right) => !Equals(left, right);

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj.GetType() == this.GetType() && this.Equals((Column)obj);
    }

    public override int GetHashCode() => this.Name?.GetHashCode() ?? 0;
}
