namespace Library.Exceptions;

[Serializable]
public sealed class ObjectNotFoundException : LibraryExceptionBase
{
    public ObjectNotFoundException()
        : this("Object")
    {
    }

    public ObjectNotFoundException(string arg)
        : base($"{arg} not found.")
    {
    }

    public ObjectNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}