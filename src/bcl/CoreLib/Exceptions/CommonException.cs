namespace Library.Exceptions;

public sealed class CommonException : LibraryExceptionBase, ILibraryException
{
    public CommonException()
    {
    }

    public CommonException(string message) : base(message)
    {
    }

    public CommonException(string message, Exception innerException) : base(message, innerException)
    {
    }

    [DoesNotReturn]
    public static void Throw(string message) =>
        throw new CommonException(message);
}