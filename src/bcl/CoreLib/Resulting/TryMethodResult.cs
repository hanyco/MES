namespace Library.Resulting;

public readonly record struct TryMethodResult(bool IsSucceed, string? Message = null, object? Error = null) : IResult
{
    public IResult? InnerResult { get; init; }

    public static explicit operator bool(TryMethodResult result) =>
        result.IsSucceed;

    public static TryMethodResult New(bool? succeed, string? message = null) =>
        new(succeed ?? true, Message: message);

    /// <summary>
    /// Creates a new TryMethodResult object with the specified value, success status, and optional message.
    /// </summary>
    public static TryMethodResult TryParseResult(bool? succeed, string? message = null) =>
        New(succeed, message: message);
}

public readonly record struct TryMethodResult<TValue>(TValue? Value,
    bool IsSucceed = true,
    string? Message = null,
    object? Error = null)
    : IResult<TValue?>
{
    public IResult? InnerResult { get; init; }

    public static explicit operator bool(TryMethodResult<TValue?> result) =>
        result.IsSucceed;

    public static explicit operator TValue?(TryMethodResult<TValue> result) =>
        result.Value;

    /// <summary>
    /// Creates a new TryMethodResult object with the specified value, success status, and optional message.
    /// </summary>
    public static TryMethodResult<TValue> TryParseResult(bool isSucceed = true, TValue? value, string? message = null) =>
        new(value, isSucceed, Message: message);
}