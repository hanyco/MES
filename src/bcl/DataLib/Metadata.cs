using System.Collections.Immutable;

namespace DataLib;

public readonly struct Database
{
    public string Name { get; init; }

    public ImmutableArray<Table> Tables { get; init; }
}

public readonly struct Table
{
    public string Name { get; init; }
    public ImmutableArray<Field> Fields { get; init; }
}

public readonly struct Field
{
    public string Name { get; init; }
    public string Type { get; init; }
}