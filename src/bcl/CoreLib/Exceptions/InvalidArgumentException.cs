namespace Library.Exceptions;

[Serializable]
public sealed class InvalidArgumentException : LibraryExceptionBase
{
    public InvalidArgumentException()
    {
    }

    public InvalidArgumentException(string message) : base(message)
    {
    }

    public InvalidArgumentException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
