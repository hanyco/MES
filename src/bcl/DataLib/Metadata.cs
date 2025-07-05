using System.Collections.Immutable;
using System.Data;
using System.Runtime.CompilerServices;

using DataLib.Extensions;

using Library.Coding;

using Microsoft.Data.SqlClient;

namespace DataLib;

public sealed class Database : IEquatable<Database>
{
    public override string ToString()
        => this.Name;
    public static async IAsyncEnumerable<Table> GetTables(string connectionString, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync(cancellationToken);
        var tables = await connection.LoadTable("""
                SELECT
                	t.object_id,
                    s.name AS [schema],
                    t.name AS [name]
                FROM sys.tables t
                LEFT JOIN sys.schemas s ON t.schema_id = s.schema_id;
                """, cancellationToken);
        var columns = await connection.LoadTable("""
                SELECT
                    c.object_id,
                    c.name AS [name],
                    c.column_id,
                    t.name AS [type],
                    c.is_nullable,
                    c.is_identity
                FROM sys.columns c
                LEFT JOIN sys.types t ON c.user_type_id = t.user_type_id;
                """, cancellationToken);
        foreach (DataRow row in tables.Rows)
        {
            yield return new Table
            {
                ObjectId = row.Field<int>("object_id")!,
                Schema = row.Field<string>("schema")!,
                Name = row.Field<string>("name")!,
                Fields = [.. columns.AsEnumerable()
                        .Where(c => c.Field<int>("object_id") == row.Field<int>("object_id"))
                        .Select(c => new Field
                        {
                            ObjectId = c.Field<int>("object_id"),
                            Name = c.Field<string>("name")!,
                            SequenceId = c.Field<int>("column_id"),
                            Type = TypePath.Parse(c.Field<string>("type")!),
                            AllowNull = c.Field<bool>("is_nullable"),
                            IsIdentity = c.Field<bool>("is_identity")
                        })]
            };
        }
    }
    public required string Name { get; init; }

    public int ObjectId { get; init; }

    public ImmutableArray<Table> Tables { get; init; }

    public static bool operator !=(Database left, Database right) => !left.Equals(right);

    public static bool operator ==(Database left, Database right) => left.Equals(right);

    public bool Equals(Database? other)
        => this.GetHashCode() == other?.GetHashCode();

    public override bool Equals(object? obj)
        => obj is Database other && this.Equals(other);

    public override int GetHashCode()
        => this.ObjectId == 0 ? this.Name.GetHashCode() : this.ObjectId.GetHashCode();
}

public sealed class Field : IEquatable<Field>
{
    public override string ToString()
        => this.Name;
    public bool AllowNull { get; init; }

    public bool IsIdentity { get; init; }

    public required string Name { get; init; }

    public int ObjectId { get; init; }

    public int SequenceId { get; init; }

    public required TypePath Type { get; init; }

    public static bool operator !=(Field left, Field right) => !left.Equals(right);

    public static bool operator ==(Field left, Field right) => left.Equals(right);

    public bool Equals(Field? other)
        => this.GetHashCode() == other?.GetHashCode();

    public override bool Equals(object? obj)
        => obj is Field other && this.Equals(other);

    public override int GetHashCode()
        => this.ObjectId == 0 ? this.Name.GetHashCode() : this.ObjectId.GetHashCode();
}

public sealed class Table : IEquatable<Table>
{
    public override string ToString()
        => this.Name;
    public ImmutableArray<Field> Fields { get; init; }

    public required string Name { get; init; }

    public int ObjectId { get; init; }

    public string? Schema { get; init; }

    public static bool operator !=(Table left, Table right) => !left.Equals(right);

    public static bool operator ==(Table left, Table right) => left.Equals(right);

    public bool Equals(Table? other)
        => this.GetHashCode() == other?.GetHashCode();

    public override bool Equals(object? obj)
        => obj is Table other && this.Equals(other);

    public override int GetHashCode()
        => this.ObjectId == 0 ? this.Name.GetHashCode() : this.ObjectId.GetHashCode();
}
