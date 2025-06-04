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
}