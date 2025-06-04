using Library.Validations;

namespace Library.DesignPatterns.StateMachine;

public static class StateMachineManager
{
    public static async Task<(TState? Last, IEnumerable<(TState State, MoveDirection Direction)> History)> Dispatch<TState>(
        [DisallowNull] Func<Task<(TState State, MoveDirection Direction)>> start,
        [DisallowNull] Func<(TState? Current, IEnumerable<(TState State, MoveDirection Direction)> History), Task<(TState State, MoveDirection Direction)>> moveNext,
        [DisallowNull] Func<(TState? Current, IEnumerable<(TState State, MoveDirection Direction)> History), Task<(TState State, MoveDirection Direction)>> moveBack,
        Func<(TState? Current, IEnumerable<(TState State, MoveDirection Direction)> History), Task>? notify = null,
        Func<(TState? Current, IEnumerable<(TState State, MoveDirection Direction)> History), Task>? done = null,
        Stack<(TState State, MoveDirection Direction)>? history = null)
    {
        Check.MustBeArgumentNotNull(start);
        Check.MustBeArgumentNotNull(moveNext);
        Check.MustBeArgumentNotNull(moveBack);

        history ??= new();
        history.Clear();
        var proc = notify ?? (_ => Task.CompletedTask);
        var current = await start();
        return (await moveAsync(current), history);

        async Task<TState> moveAsync((TState State, MoveDirection Direction) flow)
        {
            if (flow.Direction == MoveDirection.Ended)
            {
                await (done?.Invoke((flow.State, history)) ?? Task.CompletedTask);
                return flow.State;
            }
            history.Push(flow);
            await proc((flow.State, history));
            
            return flow.Direction switch
            {
                MoveDirection.Forward => await moveAsync(await moveNext((flow.State, history))),
                MoveDirection.Backward => await moveAsync(await moveBack((flow.State, history))),
                _ => throw new ArgumentOutOfRangeException(nameof(flow), flow.Direction, $"{nameof(flow.Direction)} is out of range.")
            };
        }
    }

    public static Task<(TState? Last, IEnumerable<(TState State, MoveDirection Direction)> History)> Dispatch<TState>([DisallowNull] this IAsyncStateMachine<TState> stateMachine)
        => Dispatch(
            stateMachine.ArgumentNotNull().StartAsync,
            stateMachine.MoveNextAsync,
            stateMachine.MoveBackAsync,
            stateMachine.NotifyStateAsync,
            stateMachine.DoneAsync
            );
}