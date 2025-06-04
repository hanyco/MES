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
    public static void MustBe([DoesNotReturnIf(false)] bool ok, in Func<Exception> getExceptionIfNot)
    {
        if (!ok)
        {
            throw getExceptionIfNot();
        }
    }

    public static void MustBe([DoesNotReturnIf(false)] bool ok, Func<string> onNotOk) =>
        MustBe(ok, () => new Exception(onNotOk()));

    public static void MustBe([DoesNotReturnIf(false)] bool ok, string message) =>
        MustBe(ok, () => message);

    public static void MustBe([DoesNotReturnIf(false)] bool ok, string message, params object[] args) =>
        MustBe(ok, () => string.Format(message, args));

    public static void MustBeArgumentNotNull([NotNull][AllowNull] object? obj, string? argumentName = null) =>
        MustBe(obj != null, () => new ValidationException($"{argumentName} cannot be null"));

    public static void MustBeNotNull([NotNull][AllowNull] object? obj, string? argumentName = null) =>
        MustBe(obj is not null, () => new ValidationException($"{argumentName} cannot be null"));

    extension<T>([NotNull][AllowNull] T? obj)
    {
        public T ArgumentNotNull([CallerMemberName] string? argumentName = null)
        {
            MustBeArgumentNotNull(obj, argumentName);
            return obj;
        }

        public T NotNull([CallerMemberName] string? argumentName = null)
        {
            MustBe(obj is not null, () => new ValidationException($"{argumentName} cannot be null"));
            return obj;
        }
    }
}