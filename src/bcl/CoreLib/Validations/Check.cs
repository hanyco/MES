using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Library.Exceptions;

namespace Library.Validations;

public static class Check
{
    /// <summary>
    /// Throws an exception if the specified boolean is false.
    /// </summary>
    /// <param name="ok">                The boolean to check. </param>
    /// <param name="getExceptionIfNot">
    /// A function to get the exception to throw if the boolean is false.
    /// </param>
    public static void MustBe([DoesNotReturnIf(false)] bool ok, Func<Exception> getExceptionIfNot)
    {
        if (!ok)
        {
            throw getExceptionIfNot();
        }
    }

    public static void MustBe([DoesNotReturnIf(false)] bool ok, Func<string> getMessage) =>
        MustBe(ok, () => new CommonException(getMessage()));

    public static void MustBe([NotNull][AllowNull] object? obj, string? argumentName = null) =>
        MustBe(obj != null, () => new ValidationException($"invalid value for argument: {argumentName}"));
    public static void MustBeArgumentNotNull([NotNull][AllowNull] object? obj, string? argumentName = null) =>
        MustBe(obj != null, () => new ValidationException($"{argumentName} cannot be null"));

    public static void MustBeNotNull([NotNull][AllowNull] object? obj, string? argumentName = null) =>
        MustBe(obj is not null, () => new ValidationException($"{argumentName} cannot be null"));

    public static void MustBeNotNull([NotNull][AllowNull] object? obj, Func<string> getMessage) =>
        MustBe(obj is not null, () => new ValidationException(getMessage()));

    public static void MustBeNotNull([NotNull][AllowNull] object? obj, Func<Exception> getException) =>
        MustBe(obj is not null, getException);

    public static void MustBeNotNullOrEmpty([NotNull][AllowNull] string? str, string? argumentName = null) =>
        MustBe(!string.IsNullOrEmpty(str), () => new ValidationException($"{argumentName} cannot be null or empty"));

    public static void MustBeNotNullOrEmpty([NotNull][AllowNull] string? str, Func<Exception> getException) =>
        MustBe(!string.IsNullOrEmpty(str), getException);

    extension<T>([NotNull][AllowNull] T? obj)
    {
        public T ArgumentNotNull([CallerMemberName] string? argumentName = null)
        {
            MustBeArgumentNotNull(obj, argumentName);
            return obj;
        }

        public T NotNull([CallerMemberName] string? argumentName = null)
        {
            MustBeNotNull(obj, () => new ValidationException($"{argumentName} cannot be null"));
            return obj;
        }

        public T NotNull(Func<string> getMessage)
        {
            MustBeNotNull(obj, getMessage);
            return obj;
        }
    }
}