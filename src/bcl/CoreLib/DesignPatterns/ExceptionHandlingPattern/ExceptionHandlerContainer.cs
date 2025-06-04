using Library.EventsArgs;

namespace Library.DesignPatterns.ExceptionHandlingPattern;

/// <summary>
/// ExceptionHandler Pattern Container
/// </summary>
/// <seealso cref="HanyCo.Mes20.Infra.DesignPatterns.ExceptionHandlingPattern.IExceptionHandlerContainer"/>
public class ExceptionHandlerContainer : IExceptionHandlerContainer
{
    private ExceptionHandling? _exceptionHandling;

    /// <summary>
    /// Gets or sets the exception handling.
    /// </summary>
    /// <value>The exception handling.</value>
    public virtual ExceptionHandling ExceptionHandling
    {
        get => this._exceptionHandling ??= this.OnExceptionHandlingRequired();
        protected set => this._exceptionHandling = value;
    }

    /// <summary>
    /// Called when [exception handling required].
    /// </summary>
    /// <returns></returns>
    protected virtual ExceptionHandling OnExceptionHandlingRequired()
    {
        var result = new ExceptionHandling();
        result.ExceptionOccurred += this.OnExceptionOccurred;
        return result;
    }

    /// <summary>
    /// Called when [exception occurred].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">
    /// The <see cref="HanyCo.Mes20.Infra.EventsArgs.ExceptionOccurredEventArgs{Exception}"/>
    /// instance containing the event data.
    /// </param>
    protected virtual void OnExceptionOccurred(object? sender, ExceptionOccurredEventArgs<Exception> e)
    { }
}