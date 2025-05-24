using Library.Resulting;

namespace Library.Extensions;

public static class ResultExtension
{
    /// <summary/>
    extension<TValue>(IResult<TValue> result)
    {
        /// <summary>
        /// Retrieves the value of the result if the operation was successful.
        /// </summary>
        /// <remarks>This method returns the value associated with a successful result.  If the result is
        /// null, or if the operation was not successful, an exception is thrown.</remarks>
        /// <returns>The value of the result if the operation was successful.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the result is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the result indicates a failure. The exception message will include the failure message if
        /// available.</exception>
        public TValue GetValue() =>
            result switch
            {
                null => throw new ArgumentNullException(nameof(result)),
                { IsSucceed: true } => result.Value,
                _ => throw new InvalidOperationException(result.Message ?? "Result is not successful.")
            };

        /// <summary>
        /// Determines whether the operation was successful.
        /// </summary>
        /// <returns><see langword="true"/> if the operation was successful; otherwise, <see langword="false"/>.</returns>
        public bool IsSuccessful() =>
            result != null && result.IsSucceed;
    }
}