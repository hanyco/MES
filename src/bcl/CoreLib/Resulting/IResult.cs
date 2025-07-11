namespace Library.Resulting;

public interface IResult
{
    object? Error { get; init; }
    IResult? InnerResult { get; init; }
    bool IsFailure => !this.IsSucceed;
    bool IsSucceed { get; init; }
    string? Message { get; init; }
}

public interface IResult<TValue> : IResult
{
    TValue Value { get; init; }
}

public interface IAsyncResultEnumerable : IAsyncEnumerable<IResult>;
public interface IAsyncResultEnumerable<TValue> : IAsyncEnumerable<IResult<TValue>>;