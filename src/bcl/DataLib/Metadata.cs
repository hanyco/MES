using System.Collections.Immutable;

using Library.Coding;

namespace DataLib;

public readonly struct Database
{
    public string Name { get; init; }
    public int ObjectId { get; init; }
    public ImmutableArray<Table> Tables { get; init; }
}

public readonly struct Table
{
    public ImmutableArray<Field> Fields { get; init; }
    public required string Name { get; init; }
    public int ObjectId { get; init; }
    public string Schema { get; init; }
}

public readonly struct Field
{
    public bool AllowNull { get; }
    public bool IsIdentity { get; }
    public string Name { get; init; }
    public int ObjectId { get; init; }
    public int SequenceId { get; }
    public TypePath Type { get; init; }
}