namespace Library.EventsArgs;

public abstract class ExceptionOccurredEventArgsBase<TException> : EventArgs
    where TException : Exception
{
    protected ExceptionOccurredEventArgsBase(TException exception) => this.Exception = exception;

    protected ExceptionOccurredEventArgsBase(TException exception, string? moreInfo)
        => (this.Exception, this.MoreInfo) = (exception, moreInfo);

    public bool Handled { get; set; }
    public TException Exception { get; }
    public string? MoreInfo { get; }
}
