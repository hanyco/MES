using System.Diagnostics.CodeAnalysis;

using Library.Resulting;

namespace Library.Validations;

public interface IAsyncValidatable
{
    /// <summary>
    /// Asynchronously validates the current state of the object.
    /// </summary>
    /// <param name="token">
    /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<IResult> ValidateAsync(CancellationToken token = default);
}

public interface IAsyncValidator<TItem>
{
    /// <summary>
    /// Asynchronously validates the given item and returns a result.
    /// </summary>
    /// <param name="item">The item to validate.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A result indicating the validation status.</returns>
    Task<IResult<TItem?>> ValidateAsync(TItem? item, CancellationToken token = default);
}

public interface IValidatable<T>
{
    /// <summary>
    /// Validates the current state and returns a Result object.
    /// </summary>
    IResult<T> Validate();
}

public interface IValidatable
{
    /// <summary>
    /// Validates the current state and returns a Result object.
    /// </summary>
    [return: NotNull]
    Result Validate();
}

public interface IValidator<TItem>
{
    /// <summary>
    /// Validates the given item and returns a Result object.
    /// </summary>
    Result<TItem?> Validate(in TItem? item);
}

public interface IStaticValidator<TItem>
{
    /// <summary>
    /// Validates the given item and returns a Result object.
    /// </summary>
    static abstract Result<
#nullable disable // there's no way to annotate the connection of the nullability of TItem to that of the source
                TItem
#nullable restore        
        > Validate(in TItem item);
}