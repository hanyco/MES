namespace Library.Exceptions;

public abstract class LibraryException : Exception, ILibraryException
{
    public LibraryException() : base("An error occurred in the library.")
    {
    }
    public LibraryException(string message) : base(message)
    {
    }
    public LibraryException(string message, Exception innerException) : base(message, innerException)
    {
    }
    public string? Instruction { get; init; }
}
