using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

using Library.Exceptions;
using Library.Resulting;
using Library.Validations;

namespace Library.Extensions;

[DebuggerStepThrough]
[StackTraceHidden]
public static class ResultHelper
{
    /// <summary>
    /// Breaks the execution if the given Result is not successful.
    /// </summary>
    /// <typeparam name="TResult"> </typeparam>
    /// <param name="task"> </param>
    /// <returns> </returns>
    public static async Task<TResult> BreakOnFail<TResult>(this Task<TResult> task)
        where TResult : IResult
    {
        var result = await task;
        return result.BreakOnFail();
    }

    /// <summary>
    /// Breaks the execution if the given Result is not successful.
    /// </summary>
    /// <typeparam name="TResult"> </typeparam>
    /// <param name="result"> </param>
    /// <returns> </returns>
    public static TResult BreakOnFail<TResult>(this TResult result)
        where TResult : IResult
    {
        if (result?.IsSucceed is not true)
        {
            Break();
        }
        return result;
    }

    public static void Deconstruct(this Result result, out bool isSucceed, out string message) =>
        (isSucceed, message) = (result.ArgumentNotNull().IsSucceed, result.Message?.ToString() ?? string.Empty);

    public static void Deconstruct<TValue>(this Result<TValue> result, out bool IsSucceed, out TValue Value) =>
        (IsSucceed, Value) = (result.ArgumentNotNull().IsSucceed, result.Value);

    public static void End(this IResult _)
    { }

    public static Task End(this Task<Result> _) =>
        _;

    public static Task End<T>(this Task<Result<T>> _) =>
        _;

    public static async Task<TValue> GetValueAsync<TValue>(this Task<Result<TValue>> taskResult)
    {
        var result = await taskResult;
        return result.Value;
    }

    public static TResult OnDone<TResult>([DisallowNull] this TResult result, [DisallowNull] Action<TResult> action) where TResult : IResult
    {
        Check.MustBeArgumentNotNull(action);

        action(result);
        return result;
    }

    [return: NotNullIfNotNull(nameof(Result))]
    public static TResult? OnFailure<TResult>([DisallowNull] this TResult result, [DisallowNull] Action action) where TResult : IResult
    {
        if (result?.IsSucceed == false)
        {
            action.ArgumentNotNull()();
        }

        return result;
    }

    [return: NotNullIfNotNull(nameof(Result))]
    public static TResult? OnFailure<TResult>([DisallowNull] this TResult result, [DisallowNull] Action<TResult> action) where TResult : IResult
    {
        if (result?.IsSucceed == false)
        {
            action.ArgumentNotNull()(result);
        }

        return result;
    }

    public static async Task<TResult> OnFailure<TResult>(this Task<TResult> result, [DisallowNull] Action next) where TResult : IResult
    {
        var r = await result;
        if (r.IsFailure)
        {
            next.ArgumentNotNull()();
        }

        return r;
    }

    public static async Task<TResult> OnFailure<TResult>(this Task<TResult> result, [DisallowNull] Action<TResult> next) where TResult : IResult
    {
        var r = await result;
        if (r.IsFailure)
        {
            next.ArgumentNotNull()(r);
        }

        return r;
    }

    public static async Task<TResult> OnFailure<TResult>(this Task<TResult> result, [DisallowNull] Func<TResult> next) where TResult : IResult
    {
        var r = await result;
        return r.IsFailure ? next.ArgumentNotNull()() : r;
    }

    public static async Task<TResult> OnFailure<TResult>(this Task<TResult> result, [DisallowNull] Func<TResult, TResult> next) where TResult : IResult
    {
        var r = await result;
        return r.IsFailure ? next.ArgumentNotNull()(r) : r;
    }

    //[return: NotNullIfNotNull(nameof(result))]
    //public static TResult? IfSucceed<TResult>(this TResult? result, [DisallowNull] Func<TResult> next) where TResult : IResult
    //    => result?.IsSucceed == true ? next.ArgumentNotNull()() : result;

    //public static async Task<TResult> IfSucceed<TResult>(this Task<TResult> result, [DisallowNull] Func<TResult> next) where TResult : IResult
    //{
    //    var r = await result;
    //    return r.IsSucceed ? next.ArgumentNotNull()() : r;
    //}

    [return: NotNullIfNotNull(nameof(result))]
    public static TResult? OnSucceed<TResult>(this TResult? result, [DisallowNull] Action<TResult> action) where TResult : IResult
    {
        if (result?.IsSucceed == true)
        {
            action.ArgumentNotNull()(result);
        }

        return result;
    }

    public static async Task<TResult> OnSucceed<TResult>(this Task<TResult> resultTask, [DisallowNull] Action<TResult> action) where TResult : IResult
    {
        var result = await resultTask;
        if (result?.IsSucceed == true)
        {
            action.ArgumentNotNull()(result);
        }

        return result!;
    }

    public static async Task<TResult?> OnSucceedAsync<TResult>(this TResult? result, [DisallowNull] Func<TResult, CancellationToken, Task<TResult>> next, CancellationToken token = default) where TResult : IResult
        => result?.IsSucceed == true
            ? await next.ArgumentNotNull()(result, token)
            : result;

    public static async Task<TResult> OnSucceedAsync<TResult>(this Task<TResult> result, [DisallowNull] Func<TResult, CancellationToken, Task<TResult>> next, CancellationToken token = default) where TResult : IResult
    {
        var r = await result;
        return r.IsSucceed ? await next.ArgumentNotNull()(r, token) : r;
    }

    /// <summary>
    /// Throws an exception if the given Result is not successful.
    /// </summary>
    /// <param name="result">      The Result to check. </param>
    /// <param name="owner">       The object that is throwing the exception. </param>
    /// <param name="instruction"> The instruction that is throwing the exception. </param>
    /// <returns> The given Result. </returns>
    public static Result ThrowOnFail([DisallowNull] this Result result, object? owner = null, string? instruction = null) =>
        InnerThrowOnFail(result, owner, instruction);

    /// <summary>
    /// Throws an exception if the given result is not successful.
    /// </summary>
    /// <typeparam name="TResult"> The type of the result. </typeparam>
    /// <param name="result">      The result to check. </param>
    /// <param name="owner">       The owner of the result. </param>
    /// <param name="instruction"> The instruction associated with the result. </param>
    /// <returns> The given result. </returns>
    public static TResult ThrowOnFail<TResult>([DisallowNull] this TResult result, object? owner = null, string? instruction = null) where TResult : IResult
        => InnerThrowOnFail(result, owner, instruction);

    /// <summary>
    /// Throws an exception if the given <see cref="ResultTValue" /> is a failure.
    /// </summary>
    /// <typeparam name="TValue"> The type of the value. </typeparam>
    /// <param name="result">      The <see cref="ResultTValue" /> to check. </param>
    /// <param name="owner">       The object that is responsible for the operation. </param>
    /// <param name="instruction"> The instruction that is responsible for the operation. </param>
    /// <returns> The given <see cref="ResultTValue" />. </returns>
    public static Result<TValue> ThrowOnFail<TValue>([DisallowNull] this Result<TValue> result, object? owner = null, string? instruction = null)
        => InnerThrowOnFail(result, owner, instruction);

    /// <summary>
    /// Throws an exception if the result of the provided Task is a failure.
    /// </summary>
    /// <typeparam name="TValue"> The type of the value. </typeparam>
    /// <param name="resultAsync"> The result to check. </param>
    /// <param name="owner">       The owner of the result. </param>
    /// <param name="instruction"> The instruction associated with the result. </param>
    /// <returns> The result of the provided Task. </returns>
    public static async Task<Result<TValue>> ThrowOnFailAsync<TValue>(this Task<Result<TValue>> resultAsync, object? owner = null, string? instruction = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await resultAsync;
        return InnerThrowOnFail(result, owner, instruction);
    }

    /// <summary>
    /// Throws an exception if the result of the provided Task is a failure or the task is cancelled.
    /// </summary>
    /// <typeparam name="TValue"> </typeparam>
    /// <param name="resultAsync">       </param>
    /// <param name="cancellationToken"> </param>
    /// <returns> </returns>
    public static async Task<Result<TValue>> ThrowOnFailAsync<TValue>(this Task<Result<TValue>> resultAsync, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await resultAsync;
        return InnerThrowOnFail(result, null, null);
    }

    /// <summary>
    /// Throws an exception if the result of the provided Task is a failure.
    /// </summary>
    /// <typeparam name="TResult"> </typeparam>
    /// <param name="resultAsync">       </param>
    /// <param name="owner">             </param>
    /// <param name="instruction">       </param>
    /// <param name="cancellationToken"> </param>
    /// <returns> </returns>
    public static async Task<TResult> ThrowOnFailAsync<TResult>(this Task<TResult> resultAsync, object? owner = null, string? instruction = null, CancellationToken cancellationToken = default)
        where TResult : IResult
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await resultAsync;
        return InnerThrowOnFail(result, owner, instruction);
    }

    /// <summary>
    /// Throws an exception if the result of the provided Task is a failure.
    /// </summary>
    /// <param name="resultAsync">       </param>
    /// <param name="owner">             </param>
    /// <param name="instruction">       </param>
    /// <param name="cancellationToken"> </param>
    /// <returns> </returns>
    public static async Task<Result> ThrowOnFailAsync(this Task<Result> resultAsync, object? owner = null, string? instruction = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await resultAsync;
        return InnerThrowOnFail(result, owner, instruction);
    }

    public static void ThrowOnFailOrEnd<TValue>([DisallowNull] this Result<TValue> result, object? owner = null, string? instruction = null)
                        => InnerThrowOnFail(result, owner, instruction);

    public static async Task ThrowOnFailOrEndAsync<TValue>(this Task<Result<TValue>> resultAsync, object? owner = null, string? instruction = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await resultAsync;
        InnerThrowOnFail(result, owner, instruction).End();
    }

    public static async Task ThrowOnFailOrEndAsync<TValue>(this Task<Result<TValue>> resultAsync, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await resultAsync;
        InnerThrowOnFail(result, null, null).End();
    }

    public static Task<TResult> ToAsync<TResult>(this TResult result) where TResult : IResult
        => Task.FromResult(result);

    [return: NotNull]
    public static Result<TValue> ToNotNullValue<TValue>(this Result<TValue?> result)
        where TValue : class
    {
        Check.MustBeNotNull(result.Value);
        return result!;
    }

    [return: NotNull]
    public static async Task<Result<TValue>> ToNotNullValue<TValue>(this Task<Result<TValue?>> result)
        where TValue : class
    {
        var r = await result;
        Check.MustBeNotNull(r.Value);
        return r!;
    }

    public static async Task<Result<TValue1>> ToResultAsync<TValue, TValue1>(this Task<Result<TValue>> resultTask, Func<TValue, TValue1> getNewValue)
    {
        Check.MustBeArgumentNotNull(getNewValue);

        var result = await resultTask;
        var value1 = getNewValue((TValue)result);
        return Result.From<TValue1>(result, value1);
    }

    public static async Task<Result> ToResultAsync<TValue>(this Task<Result<TValue>> resultTask)
        => await resultTask;

    /// <summary>
    /// Tries to parse the input object as a <typeparamref name="TResult" /> object and retrieves
    /// the result.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of <see cref="IResult" /> to parse the input as.
    /// </typeparam>
    /// <param name="input">  The input object to parse. </param>
    /// <param name="result">
    /// When this method returns, contains the parsed <typeparamref name="TResult" /> object if
    /// successful, or the default value if parsing fails.
    /// </param>
    /// <returns>
    /// <c> true </c> if the parsing is successful and the result is a success, <c> false </c> otherwise.
    /// </returns>
    /// <remarks>
    /// The method sets the <paramref name="result" /> parameter to the parsed object and checks if
    /// the parsing is successful by evaluating <see cref="IResult.IsSucceed" />.
    /// </remarks>
    public static bool TryParse<TResult>([DisallowNull] this TResult input, [NotNull] out TResult result) where TResult : IResult =>
        (result = input).IsSucceed;

    //! Compiler Error CS1988: Async methods cannot have `ref`, `in` or `out` parameters
    //x public static async Task<bool> TryParseAsync<TResult>([DisallowNull] this Task<TResult> input, out TResult result) where TResult : IResult
    //x     => (result = await input).IsSucceed;

    public static Result<TNewValue> WithValue<TNewValue>(this IResult result, in TNewValue newValue) =>
        new(result, newValue);

    /// <summary>
    /// Creates a new instance of the <see cref="Result{TValue}" /> class with the specified value.
    /// </summary>
    /// <param name="value"> The value to set. </param>
    /// <returns>
    /// A new instance of the <see cref="Result{TValue}" /> class with the specified value.
    /// </returns>
    public static Result<TValue> WithValue<TValue>(this Result<TValue> result, in TValue value) =>
        new(result, value);

    private static TResult InnerThrowOnFail<TResult>([DisallowNull] TResult result, object? owner, string? instruction = null)
        where TResult : IResult
    {
        Check.MustBeArgumentNotNull(result);
        if (result.IsSucceed)
        {
            return result;
        }

        Exception exception;
        var error = result.Error;

        if (!result.Message.IsNullOrEmpty())
        {
            exception = new CommonException(result.Message) { Instruction = instruction }.With(x => x.Source = owner?.ToString());
        }
        else if (error is Exception ex)
        {
            exception = ex.With(x => x.Source = owner?.ToString());
        }
        else
        {
            exception = new CommonException("An unknown error occurred.") { Instruction = instruction }.With(x => x.Source = owner?.ToString());
        }
        ExceptionDispatchInfo.Throw(exception);
        return result;
    }
}