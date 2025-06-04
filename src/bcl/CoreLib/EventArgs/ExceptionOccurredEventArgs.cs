namespace Library.EventsArgs;

public sealed class ExceptionOccurredEventArgs<TException> : ExceptionOccurredEventArgsBase<TException>
    where TException : Exception
{
    public ExceptionOccurredEventArgs(TException exception)
        : base(exception)
    {
    }

    public ExceptionOccurredEventArgs(TException exception, string? moreInfo)
        : base(exception, moreInfo)
    {
    }
}

public sealed class ExceptionOccurredEventArgs : ExceptionOccurredEventArgsBase<Exception>
{
    public ExceptionOccurredEventArgs(Exception exception)
        : base(exception)
    {
    }

    public ExceptionOccurredEventArgs(Exception exception, string? moreInfo)
        : base(exception, moreInfo)
    {
    }
}
