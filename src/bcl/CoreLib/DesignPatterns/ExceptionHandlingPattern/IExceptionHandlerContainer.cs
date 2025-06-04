namespace Library.DesignPatterns.ExceptionHandlingPattern;

/// <summary>
/// </summary>
/// <typeparam name="TException">The type of the exception.</typeparam>
public interface IExceptionHandlerContainer<TException>
    where TException : Exception
{
    /// <summary>
    /// Gets the exception handling.
    /// </summary>
    /// <value>The exception handling.</value>
    ExceptionHandling<TException> ExceptionHandling { get; }
}

/// <summary>
/// </summary>
public interface IExceptionHandlerContainer
{
    /// <summary>
    /// Gets the exception handling.
    /// </summary>
    /// <value>The exception handling.</value>
    ExceptionHandling ExceptionHandling { get; }
}