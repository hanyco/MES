namespace Library.Resulting;

public readonly record struct Result(bool IsSucceed, string? Message = null, object? Error = null) : IResult
{
    public static implicit operator Result(bool isSucceed) =>
        new(isSucceed, null);

    public static Result<TValue> From<TValue>(IResult result, TValue value, bool isSucceedDefault = true) =>
        new() { Value = value, IsSucceed = result?.IsSucceed ?? isSucceedDefault, Message = result?.Message };

    public static Result Success() =>
        new() { IsSucceed = true };

    public static Result Fail(object error) =>
        new(false, null, error);

    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true);

    public static Result<TValue> Fail<TValue>(TValue value, object error) =>
        new(value, false, null, error);

    public static Result<TValue?> Fail<TValue>(object error) =>
        new(default, false, null, error);

    public static Result<TValue> Fail<TValue>(TValue value, string? message, object error) =>
        new(value, false, message, error);
}
public readonly record struct Result<TValue>(TValue Value, bool IsSucceed = true, string? Message = null, object? Error = null) : IResult<TValue>
{
    public static explicit operator TValue(Result<TValue> result) =>
        result.Value;

    public Result(IResult result, TValue value)
        : this(value, result.IsSucceed, result.Message, result.Error)
    {
    }
    
    public static implicit operator Result(Result<TValue> result) =>
        new(result.IsSucceed, result.Message, result.Error);
}