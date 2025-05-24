namespace Library.Resulting;

public readonly record struct Result(bool IsSucceed, string? Message = null) : IResult;
public readonly record struct Result<TValue>(TValue Value, bool IsSucceed, string? Message = null) : IResult<TValue>;