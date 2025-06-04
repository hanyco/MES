
namespace Library.Exceptions;

public class ValidationException : LibraryExceptionBase, IValidationException
{
    public ValidationException()
    {
    }

    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public static void Throw(string message) =>
        throw new ValidationException(message);
}