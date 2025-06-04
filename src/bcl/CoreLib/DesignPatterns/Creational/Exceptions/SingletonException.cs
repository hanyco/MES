using Library.Exceptions;

namespace Library.DesignPatterns.Creational.Exceptions;

/// <summary>
/// Singleton Exception
/// </summary>
/// <seealso cref="Library.Exceptions.LibraryExceptionBase"/>
[Serializable]
public sealed class SingletonException : LibraryExceptionBase
{
    public SingletonException()
    {
    }

    public SingletonException(string message) : base(message)
    {
    }

    public SingletonException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public SingletonException(string message, string? instruction = null, string? title = null, string? details = null, Exception? inner = null, object? owner = null) : base(message, instruction, title, details, inner, owner)
    {
    }
}