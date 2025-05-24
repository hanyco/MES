using System.Diagnostics.CodeAnalysis;

namespace Library.Validations;

public static class Check
{
    /// <summary>
    /// Throws an exception if the specified boolean is false.
    /// </summary>
    /// <param name="ok">The boolean to check.</param>
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
        MustBe(obj != null, () => $"{argumentName} cannot be null.");

    extension<T>([DoesNotReturnIf(false)] T obj)
    {
        public T ArgumentNotNull(string? argumentName = null)
        {
            MustBeArgumentNotNull(obj, argumentName);
            return obj;
        }
    }
}