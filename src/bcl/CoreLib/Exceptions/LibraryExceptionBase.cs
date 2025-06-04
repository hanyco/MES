namespace Library.Exceptions;

public abstract class LibraryExceptionBase : Exception, ILibraryException
{
    public LibraryExceptionBase() : base("An error occurred in the library.")
    {
    }
    public LibraryExceptionBase(string message) : base(message)
    {
    }
    public LibraryExceptionBase(string message, Exception innerException) : base(message, innerException)
    {
    }
    public string? Instruction { get; init; }
}
