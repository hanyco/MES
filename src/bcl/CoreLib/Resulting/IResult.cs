namespace Library.Resulting;

public interface IResult
{
    bool IsFailure => !this.IsSucceed;
    bool IsSucceed { get; init; }
    string? Message { get; init; }
}

public interface IResult<TValue> : IResult
{
    TValue Value { get; init; }
}