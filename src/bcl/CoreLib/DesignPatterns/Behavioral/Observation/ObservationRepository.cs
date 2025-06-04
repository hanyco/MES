using Library.EventsArgs;
using Library.Interfaces;
using Library.Validations;

namespace Library.DesignPatterns.Behavioral.Observation;

public sealed class ObservationRepository : IObservableRepository, IObserverRepository
{
    private readonly List<RegisterObservable> _observables = new();
    private readonly List<RegisterObserver> _observers = new();

    public void Add(in RegisterObservable observable)
    {
        this._observables.Add(observable.ArgumentNotNull(nameof(observable)));
        observable.Observable.Changed += this.Subject_PropertyChanged;
    }

    public void Add(in RegisterObserver observer)
        => this._observers.Add(observer);

    private void Subject_PropertyChanged(object? sender, ItemActedEventArgs<NotifyPropertyChanged> e)
    {
        Check.MustBeArgumentNotNull(e?.Item);
        if (e?.Item is null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        foreach (var (observer, propertyType, onPropertyChanged, propertyName, typeOfObservable) in this._observers)
        {
            if (typeOfObservable is not null && typeOfObservable != e.Item.Sender?.GetType())
            {
                continue;
            }
            if (propertyType is not null && propertyType != e.Item.PropertyType)
            {
                continue;
            }
            if (propertyName is not null && propertyName != e.Item.PropertyName)
            {
                continue;
            }

            onPropertyChanged?.Invoke(e.Item);
        }
    }
}