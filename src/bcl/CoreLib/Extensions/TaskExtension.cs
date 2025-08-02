namespace Library.Extensions;

public static class TaskExtension
{
    public static async Task ThrowIfCancellationRequested(this Task taskAction, CancellationToken cancellationToken = default)
    {
        await taskAction;
        cancellationToken.ThrowIfCancellationRequested();
    }

    public static async Task<TResult> ThrowIfCancellationRequested<TResult>(this Task<TResult> taskAction, CancellationToken cancellationToken = default)
    {
        var result = await taskAction;
        cancellationToken.ThrowIfCancellationRequested();
        return result;
    }
}