using System.Diagnostics;

using Library.EventsArgs;

namespace Library.DesignPatterns.ExceptionHandlingPattern;

/// <summary>
/// ExceptionHandling Pattern
/// </summary>
/// <typeparam name="TException">The type of the exception.</typeparam>
public abstract class ExceptionHandling<TException>()
    where TException : Exception
{
    /// <summary>
    /// Occurs when [exception occurred].
    /// </summary>
    public event EventHandler<ExceptionOccurredEventArgs<TException>>? ExceptionOccurred;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandling{TException}"/> class.
    /// </summary>
    /// <param name="exceptionOccurredHandler">The exception occurred handler.</param>
    public ExceptionHandling(EventHandler<ExceptionOccurredEventArgs<TException>> exceptionOccurredHandler)
        : this() => this.ExceptionOccurred += exceptionOccurredHandler;

    /// <summary>
    /// Gets a value indicating whether this instance has exception.
    /// </summary>
    /// <value><c>true</c> if this instance has exception; otherwise, <c>false</c>.</value>
    public bool HasException => this.LastException is not null;

    /// <summary>
    /// Gets the last exception.
    /// </summary>
    /// <value>The last exception.</value>
    public TException? LastException { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether [raise exceptions].
    /// </summary>
    /// <value><c>true</c> if [raise exceptions]; otherwise, <c>false</c>.</value>
    public bool RaiseExceptions { get; set; }

    private object? Sender { get; set; }

    /// <summary>
    /// Handles the exception.
    /// </summary>
    /// <param name="ex">The ex.</param>
    internal void HandleException(TException ex)
        => this.HandleException(this.Sender, ex);

    /// <summary>
    /// Handles the exception.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="ex">The ex.</param>
    internal void HandleException(object? sender, TException ex)
        => this.HandleException(sender, ex, string.Empty);

    /// <summary>
    /// Handles the exception.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="ex">The ex.</param>
    /// <param name="moreInfo">The more information.</param>
    internal void HandleException(object? sender, TException ex, string moreInfo)
    {
        Debug.WriteLine($"{moreInfo} [{ex.GetBaseException().Message}]");
        this.LastException = ex;
        this.ExceptionOccurred?.Invoke(sender, new ExceptionOccurredEventArgs<TException>(ex, moreInfo));
        if (this.RaiseExceptions)
        {
            throw ex;
        }

        //CodeHelper.Break();
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    internal void Reset()
        => this.LastException = null;

    /// <summary>
    /// Sets the sender.
    /// </summary>
    /// <param name="sender">The sender.</param>
    internal void SetSender(object sender)
        => this.Sender = sender;
}

/// <summary>
/// Exception Handling Pattern
/// </summary>
public sealed class ExceptionHandling : ExceptionHandling<Exception>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandling"/> class.
    /// </summary>
    /// <param name="exceptionOccurredHandler">The exception occurred handler.</param>
    public ExceptionHandling(EventHandler<ExceptionOccurredEventArgs<Exception>> exceptionOccurredHandler)
        : base(exceptionOccurredHandler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandling"/> class.
    /// </summary>
    public ExceptionHandling()
    { }
}