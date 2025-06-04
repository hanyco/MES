namespace Library.DesignPatterns.StateMachine;

public enum MoveDirection
{
    Forward,
    Backward,
    Ended
}

public interface IAsyncStateMachine<TState>
{
    Task<TState?> DoneAsync((TState? Current, IEnumerable<(TState State, MoveDirection Direction)> History) lastState);

    Task<(TState State, MoveDirection Direction)> MoveBackAsync((TState? Current, IEnumerable<(TState State, MoveDirection Direction)> History) current);

    Task<(TState State, MoveDirection Direction)> MoveNextAsync((TState? Current, IEnumerable<(TState State, MoveDirection Direction)> History) current);

    Task NotifyStateAsync((TState? Current, IEnumerable<(TState State, MoveDirection Direction)> History) state);

    Task<(TState State, MoveDirection Direction)> StartAsync();
}