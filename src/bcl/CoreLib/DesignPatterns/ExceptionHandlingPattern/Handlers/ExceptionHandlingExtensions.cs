namespace Library.DesignPatterns.ExceptionHandlingPattern.Handlers;

/// <summary>
/// Extensions for ExceptionHandling Pattern
/// </summary>
public static class ExceptionHandlingExtensions
{
    /// <summary>
    /// Handles the exception.
    /// </summary>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    /// <param name="exceptionHandling">The exception handling.</param>
    /// <param name="ex">The ex.</param>
    public static void HandleException<TException>(this ExceptionHandling<TException> exceptionHandling, TException ex)
        where TException : Exception => exceptionHandling.HandleException(ex);

    /// <summary>
    /// Handles the exception.
    /// </summary>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    /// <param name="exceptionHandling">The exception handling.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="ex">The ex.</param>
    public static void HandleException<TException>(this ExceptionHandling<TException> exceptionHandling, object sender, TException ex)
        where TException : Exception => exceptionHandling.HandleException(sender, ex);

    /// <summary>
    /// Resets the specified exception handling.
    /// </summary>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    /// <param name="exceptionHandling">The exception handling.</param>
    public static void Reset<TException>(this ExceptionHandling<TException> exceptionHandling)
        where TException : Exception => exceptionHandling.Reset();

    /// <summary>
    /// Sets the sender.
    /// </summary>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    /// <param name="exceptionHandling">The exception handling.</param>
    /// <param name="sender">The sender.</param>
    public static void SetSender<TException>(ExceptionHandling<TException> exceptionHandling, object sender)
        where TException : Exception => exceptionHandling.SetSender(sender);
}