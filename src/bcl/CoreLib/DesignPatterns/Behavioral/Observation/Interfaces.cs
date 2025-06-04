using Library.Interfaces;

namespace Library.DesignPatterns.Behavioral.Observation;

public sealed record RegisterObservable(in IPropertyChangeRepositoryNotifier Observable, in Type PropertyType, in string PropertyName);
public sealed record RegisterObserver(in IObserver Observer, in string? PropertyType, Action<NotifyPropertyChanged> PropertyChanged, in string? PropertyName = null, in Type? TypeOfObserable = null);

public interface IObservableRepository
{
    void Add(in RegisterObservable observable);
}

public interface IObserver
{
    void RegisterOnRepository(in IObserverRepository repository);
}

public interface IPropertyChangeRepositoryNotifier : IChangeNotifierProperty
{
    void RegisterOnRepository(in IObservableRepository repository);
}

public interface IObserverRepository
{
    void Add(in RegisterObserver observer);
}