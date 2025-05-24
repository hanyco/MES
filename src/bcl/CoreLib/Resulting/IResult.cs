namespace Library.Resulting;

public interface IResult
{
    bool IsSucceed { get; init; }
    string? Message { get; init; }

    static IResult<TValue> From<TValue>(IResult result, TValue value, bool isSucceedDefault = true) =>
        new Result<TValue> { Value = value, IsSucceed = result?.IsSucceed ?? isSucceedDefault, Message = result?.Message };
}

public interface IResult<TValue> : IResult
{
    TValue Value { get; init; }
}