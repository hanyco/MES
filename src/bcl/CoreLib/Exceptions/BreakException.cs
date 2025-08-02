namespace Library.Exceptions;

public sealed class BreakException : Exception
{
    [DoesNotReturn]
    public static void Throw() =>
        throw new BreakException();

    [DoesNotReturn]
    public static TFakeResult Throw<TFakeResult>() =>
        throw new BreakException();
}