namespace Library.Casting;

/// <summary>
/// Defines a contract for types that can be cast to an object. Provides a nullable property 'Value'
/// to retrieve the casted object.
/// </summary>
public interface ICastable
{
    object? Value { get; }
}

/// <summary>
/// An interface that provides a nullable property to retrieve a value of a specified type.
/// </summary>
/// <typeparam name="T">
/// The type of value that can be retrieved, allowing for flexibility in the data type used.
/// </typeparam>
public interface ICastable<T>
{
    T? Value { get; }
}

internal readonly record struct Castable(object? Value) : ICastable { }
internal readonly record struct Castable<T>(T? Value) : ICastable<T> { }