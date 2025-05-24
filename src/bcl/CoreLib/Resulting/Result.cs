namespace Library.Resulting;

internal readonly record struct Result(bool IsSucceed, string? Message = null) : IResult;
internal readonly record struct Result<TValue>(TValue Value, bool IsSucceed, string? Message = null) : IResult<TValue>;