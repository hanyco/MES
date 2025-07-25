using System.Runtime.CompilerServices;

using Library.Exceptions;

namespace Library.Validations;

public static class Check
{
    /// <summary>
    /// Ensures that the specified argument is not null, throwing an exception if it is.
    /// </summary>
    /// <remarks>This method is typically used to validate method arguments and ensure they are not null
    /// before proceeding. The <paramref name="argumentName"/> parameter is automatically populated with the name of the
    /// calling member if not explicitly provided, aiding in debugging and exception messages.</remarks>
    /// <typeparam name="T">The type of the argument being validated.</typeparam>
    /// <param name="obj">The argument to validate. Can be null if <typeparamref name="T"/> is a nullable type.</param>
    /// <param name="argumentName">The name of the argument being validated. Defaults to the name of the calling member.</param>
    /// <returns>The validated argument if it is not null.</returns>
    public static T EnsureArgumentNotNull<T>([NotNull][AllowNull] this T? obj, [CallerMemberName] string? argumentName = null)
    {
        MustBeArgumentNotNull(obj, argumentName);
        return obj;
    }

    /// <summary>
    /// Ensures that the specified object is not null, throwing an exception if it is.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    /// <param name="obj">The object to check for null. This parameter must not be null.</param>
    /// <param name="argumentName">The name of the argument being validated. Defaults to the caller's member name if not specified.</param>
    /// <returns>The validated object, guaranteed to be non-null.</returns>
    [return: NotNull]
    public static T EnsureNotNull<T>([NotNull][AllowNull] this T? obj, [CallerMemberName] string? argumentName = null)
    {
        MustBeNotNull(obj, () => new ValidationException($"{argumentName} cannot be null"));
        return obj;
    }

    [return: NotNull]
    public static T EnsureNotNull<T>([NotNull][AllowNull] this T? obj, Func<string> getMessage)
    {
        MustBeNotNull(obj, getMessage);
        return obj;
    }

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
}