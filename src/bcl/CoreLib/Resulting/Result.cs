namespace Library.Resulting;

public readonly record struct Result(bool IsSucceed, string? Message = null, object? Error = null) : IResult
{
    public IResult? InnerResult { get; init; }

    public static implicit operator Result(bool isSucceed) =>
        new(IsSucceed: isSucceed);
}

public readonly record struct Result<TValue>(TValue Value, bool IsSucceed = true, string? Message = null, object? Error = null) : IResult<TValue>
{
    public IResult? InnerResult { get; init; }

    public Result(IResult result, TValue value)
     : this(Value: value, IsSucceed: result.IsSucceed, Message: result.Message, Error: result.Error)
    {
    }

    public static explicit operator TValue(Result<TValue> result) =>
        result.Value;

    public static implicit operator Result(Result<TValue> result) =>
        new(IsSucceed: result.IsSucceed, Message: result.Message, Error: result.Error);
}