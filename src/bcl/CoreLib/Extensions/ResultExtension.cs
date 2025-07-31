using System.Runtime.ExceptionServices;

using Library.Exceptions;

namespace Library.Extensions;

[DebuggerStepThrough]
[StackTraceHidden]
public static class ResultExtension
{
    extension(IResult @this)
    {
        public void End()
        { }

        public Result<TNewValue> WithValue<TNewValue>(in TNewValue newValue) =>
            new(@this, newValue);
    }

    extension<TResult>(TResult @this)
        where TResult : IResult
    {
        /// <summary>
        /// Breaks the execution if the given Result is not successful.
        /// </summary>
        /// <typeparam name="TResult"> </typeparam>
        /// <param name="result"> </param>
        /// <returns> </returns>
        public TResult BreakOnFail()
        {
            if (@this?.IsSucceed is not true)
            {
                Break();
            }
            return @this;
        }

        [return: NotNullIfNotNull(nameof(@this))]
        public TResult? OnSucceed([DisallowNull] Action<TResult> action)
        {
            if (@this?.IsSucceed == true)
            {
                action.EnsureArgumentNotNull()(@this);
            }

            return @this;
        }

        public TResult OnDone([DisallowNull] Action<TResult> action)
        {
            Check.MustBeArgumentNotNull(action);

            action(@this);
            return @this;
        }

        [return: NotNullIfNotNull(nameof(Result))]
        public TResult? OnFailure([DisallowNull] Action action)
        {
            if (@this?.IsSucceed == false)
            {
                action.EnsureArgumentNotNull()();
            }

            return @this;
        }

        [return: NotNullIfNotNull(nameof(Result))]
        public TResult? OnFailure([DisallowNull] Action<TResult> action)
        {
            if (@this?.IsSucceed == false)
            {
                action.EnsureArgumentNotNull()(@this);
            }

            return @this;
        }

        /// <summary>
        /// Throws an exception if the given result is not successful.
        /// </summary>
        /// <typeparam name="TResult"> The type of the result. </typeparam>
        /// <param name="result">      The result to check. </param>
        /// <param name="owner">       The owner of the result. </param>
        /// <param name="instruction"> The instruction associated with the result. </param>
        /// <returns> The given result. </returns>
        public TResult ThrowOnFail(object? owner, string? instruction = null)
            => InnerThrowOnFail(@this, owner, instruction);
        public TResult ThrowOnFail(string? instruction = null)
            => InnerThrowOnFail(@this, null, instruction);
        public TResult ThrowOnFail()
            => InnerThrowOnFail(@this, null, null);

        /// <summary>
        /// Tries to parse the input object as a <typeparamref name="TResult" /> object and
        /// retrieves the result.
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
        /// The method sets the <paramref name="result" /> parameter to the parsed object and checks
        /// if the parsing is successful by evaluating <see cref="IResult.IsSucceed" />.
        /// </remarks>
        public bool IfSucceed([NotNull] out TResult output) =>
            (output = @this).IsSucceed;
    }

    extension<TValue>(IResult<TValue> @this)
    {
        /// <summary>
        /// Retrieves the value of the result if the operation was successful.
        /// </summary>
        /// <remarks>
        /// This method returns the value associated with a successful result. If the result is
        /// null, or if the operation was not successful, an exception is thrown.
        /// </remarks>
        /// <returns> The value of the result if the operation was successful. </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the result is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the result indicates a failure. The exception message will include the failure
        /// message if available.
        /// </exception>
        public TValue GetValue() =>
            @this switch
            {
                null => throw new ArgumentNullException(nameof(@this)),
                { IsSucceed: true } => @this.Value,
                _ => throw new InvalidOperationException(@this.Message ?? "Result is not successful.")
            };

        /// <summary>
        /// Determines whether the operation was successful.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if the operation was successful; otherwise, <see
        /// langword="false" />.
        /// </returns>
        public bool IsSuccessful() =>
            @this != null && @this.IsSucceed;

        public void Deconstruct(out bool IsSucceed, out TValue Value) =>
            (IsSucceed, Value) = (@this.EnsureArgumentNotNull().IsSucceed, @this.Value);
    }

    extension(Result @this)
    {
        public void Deconstruct(out bool isSucceed, out string message) =>
            (isSucceed, message) = (@this.EnsureArgumentNotNull().IsSucceed, @this.Message?.ToString() ?? string.Empty);

        /// <summary>
        /// Throws an exception if the given Result is not successful.
        /// </summary>
        /// <param name="result">      The Result to check. </param>
        /// <param name="owner">       The object that is throwing the exception. </param>
        /// <param name="instruction"> The instruction that is throwing the exception. </param>
        /// <returns> The given Result. </returns>
        public Result ThrowOnFail(object? owner = null, string? instruction = null) =>
            InnerThrowOnFail(@this, owner, instruction);
    }

    extension<TValue>(Result<TValue> @this)
    {
        /// <summary>
        /// Throws an exception if the given <see cref="ResultTValue" /> is a failure.
        /// </summary>
        /// <typeparam name="TValue"> The type of the value. </typeparam>
        /// <param name="result">      The <see cref="ResultTValue" /> to check. </param>
        /// <param name="owner">       The object that is responsible for the operation. </param>
        /// <param name="instruction"> The instruction that is responsible for the operation. </param>
        /// <returns> The given <see cref="ResultTValue" />. </returns>
        public Result<TValue> ThrowOnFail(object? owner = null, string? instruction = null)
            => InnerThrowOnFail(@this, owner, instruction);

        /// <summary>
        /// Creates a new instance of the <see cref="Result{TValue}" /> class with the specified value.
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <returns>
        /// A new instance of the <see cref="Result{TValue}" /> class with the specified value.
        /// </returns>
        public Result<TValue> WithValue(in TValue value) =>
            new(@this, value);

        public void ThrowOnFailOrEnd(object? owner = null, string? instruction = null)
            => InnerThrowOnFail(@this, owner, instruction);
    }

    extension(Result)
    {
        public static Result<TValue> From<TValue>(IResult result, TValue value, bool isSucceedDefault = true) =>
            new(Value: value, IsSucceed: result?.IsSucceed ?? isSucceedDefault, Message: result?.Message, Error: result?.Error) { InnerResult = result };

        public static Result Success() =>
            new(IsSucceed: true);

        public static Result Fail(object error) =>
            new(IsSucceed: false, Message: null, Error: error);

        public static Result<TValue> Success<TValue>(TValue value) =>
            new(Value: value, IsSucceed: true);

        public static Result<TValue> Fail<TValue>(TValue value, object error) =>
            new(Value: value, IsSucceed: false, Message: null, Error: error);

        public static Result<TValue?> Fail<TValue>(object error) =>
            new(Value: default, IsSucceed: false, Message: null, Error: error);

        public static Result<TValue> Fail<TValue>(TValue value, string? message, object error) =>
            new(Value: value, IsSucceed: false, Message: message, Error: error);
    }

    extension<TValue>(Result<TValue>)
    {
    }

    /// <summary>
    /// Breaks the execution if the given Result is not successful.
    /// </summary>
    /// <typeparam name="TResult"> </typeparam>
    /// <param name="this"> </param>
    /// <returns> </returns>
    public static async Task<TResult> BreakOnFail<TResult>(this Task<TResult> @this)
        where TResult : IResult
    {
        var result = await @this;
        return result.BreakOnFail();
    }

    public static async Task<TValue> GetValue<TValue>(this Task<IResult<TValue>> @this)
    {
        var result = await @this;
        return result.Value;
    }

    public static async Task<TResult> OnFailure<TResult>(this Task<TResult> @this, [DisallowNull] Action next) where TResult : IResult
    {
        var r = await @this;
        if (r.IsFailure)
        {
            next.EnsureArgumentNotNull()();
        }

        return r;
    }

    public static async Task<TResult> OnFailure<TResult>(this Task<TResult> @this, [DisallowNull] Action<TResult> next) where TResult : IResult
    {
        var r = await @this;
        if (r.IsFailure)
        {
            next.EnsureArgumentNotNull()(r);
        }

        return r;
    }

    public static async Task<TResult> OnFailure<TResult>(this Task<TResult> @this, [DisallowNull] Func<TResult> next) where TResult : IResult
    {
        var r = await @this;
        return r.IsFailure ? next.EnsureArgumentNotNull()() : r;
    }

    public static async Task<TResult> OnFailure<TResult>(this Task<TResult> @this, [DisallowNull] Func<TResult, TResult> next) where TResult : IResult
    {
        var r = await @this;
        return r.IsFailure ? next.EnsureArgumentNotNull()(r) : r;
    }

    public static async Task<TResult> OnSucceed<TResult>(this Task<TResult> @this, [DisallowNull] Action<TResult> action) where TResult : IResult
    {
        var result = await @this;
        if (result?.IsSucceed == true)
        {
            action.EnsureArgumentNotNull()(result);
        }

        return result!;
    }

    public static async Task<TResult?> OnSucceed<TResult>(this TResult? @this, [DisallowNull] Func<TResult, CancellationToken, Task<TResult>> next, CancellationToken token = default) where TResult : IResult
        => @this?.IsSucceed == true
            ? await next.EnsureArgumentNotNull()(@this, token)
            : @this;

    public static async Task<TResult> OnSucceed<TResult>(this Task<TResult> @this, [DisallowNull] Func<TResult, CancellationToken, Task<TResult>> next, CancellationToken token = default) where TResult : IResult
    {
        var r = await @this;
        return r.IsSucceed ? await next.EnsureArgumentNotNull()(r, token) : r;
    }

    /// <summary>
    /// Executes the specified function if the result of the preceding task is successful.
    /// </summary>
    /// <typeparam name="TValue"> The type of the value contained in the result. </typeparam>
    /// <typeparam name="TOutput">
    /// The type of the output produced by the <paramref name="next" /> function.
    /// </typeparam>
    /// <param name="this"> The task representing an asynchronous operation that produces a result. </param>
    /// <param name="next">
    /// A function to execute if the result is successful. The function receives the value of the
    /// result as input.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. If the result is successful, the task
    /// returns the output of the <paramref name="next" /> function. Otherwise, it returns the
    /// default value for <typeparamref name="TOutput" />.
    /// </returns>
    public static async Task<TOutput> OnSucceed<TValue, TOutput>(this Task<IResult<TValue>> @this, Func<TValue, TOutput> next)
    {
        var r = await @this;
        return r.IsSucceed ? next.EnsureArgumentNotNull()(r.Value) : default!;
    }

    /// <summary>
    /// Throws an exception if the result of the provided Task is a failure.
    /// </summary>
    /// <typeparam name="TValue"> The type of the value. </typeparam>
    /// <param name="this">        The result to check. </param>
    /// <param name="owner">       The owner of the result. </param>
    /// <param name="instruction"> The instruction associated with the result. </param>
    /// <returns> The result of the provided Task. </returns>
    public static async Task<Result<TValue>> ThrowOnFail<TValue>(this Task<Result<TValue>> @this, object? owner = null, string? instruction = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await @this;
        return InnerThrowOnFail(result, owner, instruction);
    }

    public static Task End<TResult>(this Task<TResult> resultTask)
        where TResult : IResult => resultTask;

    /// <summary>
    /// Throws an exception if the result of the provided Task is a failure or the task is cancelled.
    /// </summary>
    /// <typeparam name="TValue"> </typeparam>
    /// <param name="this">              </param>
    /// <param name="cancellationToken"> </param>
    /// <returns> </returns>
    public static async Task<Result<TValue>> ThrowOnFail<TValue>(this Task<Result<TValue>> @this, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await @this;
        return InnerThrowOnFail(result, null, null);
    }

    /// <summary>
    /// Throws an exception if the result of the provided Task is a failure.
    /// </summary>
    /// <typeparam name="TResult"> </typeparam>
    /// <param name="this">              </param>
    /// <param name="owner">             </param>
    /// <param name="instruction">       </param>
    /// <param name="cancellationToken"> </param>
    /// <returns> </returns>
    public static async Task<TResult> ThrowOnFail<TResult>(this Task<TResult> @this, object? owner = null, string? instruction = null, CancellationToken cancellationToken = default)
        where TResult : IResult
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await @this;
        return InnerThrowOnFail(result, owner, instruction);
    }

    public static Task<TValue> ParseValue<TValue>(this Task<IResult<TValue>> result)
        => result.ThrowOnFail().GetValue();

    /// <summary>
    /// Throws an exception if the result of the provided Task is a failure.
    /// </summary>
    /// <param name="this">              </param>
    /// <param name="owner">             </param>
    /// <param name="instruction">       </param>
    /// <param name="cancellationToken"> </param>
    /// <returns> </returns>
    public static async Task<Result> ThrowOnFail(this Task<Result> @this, object? owner = null, string? instruction = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await @this;
        return InnerThrowOnFail(result, owner, instruction);
    }

    public static async Task ThrowOnFailOrEndAsync<TValue>(this Task<Result<TValue>> @this, object? owner = null, string? instruction = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await @this;
        InnerThrowOnFail(result, owner, instruction).End();
    }

    public static async Task ThrowOnFailOrEndAsync<TValue>(this Task<Result<TValue>> @this, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await @this;
        InnerThrowOnFail(result, null, null).End();
    }

    public static Task<TResult> ToAsync<TResult>(this TResult @this) where TResult : IResult
        => Task.FromResult(@this);

    public static async Task<TValue> ToAsync<TValue>(this Task<IResult<TValue>> @this)
    {
        var result = await @this;
        return result.GetValue();
    }

    public static async Task<TValue> ToAsync<TResult, TValue>(this Task<TResult> @this)
        where TResult : IResult<TValue>
    {
        var result = await @this;
        return result.GetValue();
    }

    public static async Task<Result<TValue1>> ToResultAsync<TValue, TValue1>(this Task<Result<TValue>> @this, Func<TValue, TValue1> getNewValue)
    {
        Check.MustBeArgumentNotNull(getNewValue);

        var result = await @this;
        var value1 = getNewValue((TValue)result);
        return Result.From<TValue1>(result, value1);
    }

    public static async Task<Result> ToResultAsync<TValue>(this Task<Result<TValue>> @this)
        => await @this;

    private static TResult InnerThrowOnFail<TResult>([DisallowNull] in TResult result, object? owner, string? instruction = null)
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