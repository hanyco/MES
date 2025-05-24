namespace Library.Resulting;

public interface IResult
{
    bool IsSucceed { get; init; }
    string? Message { get; init; }
}

public interface IResult<TValue> : IResult
{
    TValue Value { get; init; }
}