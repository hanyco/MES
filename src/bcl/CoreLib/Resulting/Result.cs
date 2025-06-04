namespace Library.Resulting;

public readonly record struct Result(bool IsSucceed, string? Message = null) : IResult
{
    public static implicit operator Result(bool isSucceed) =>
        new(isSucceed, null);

    public static Result<TValue> From<TValue>(IResult result, TValue value, bool isSucceedDefault = true) =>
        new() { Value = value, IsSucceed = result?.IsSucceed ?? isSucceedDefault, Message = result?.Message };

    public static Result Succeed =>
        new() { IsSucceed = true };
}
public readonly record struct Result<TValue>(TValue Value, bool IsSucceed = true, string? Message = null) : IResult<TValue>
{
    public static explicit operator TValue(Result<TValue> result) =>
        result.Value;
}