using Library.EventsArgs;

namespace Library.DesignPatterns.Behavioral.Observation;

public sealed class ObservableObject<TValue>
{
    public event EventHandler<ItemActingEventArgs<TValue?>>? Changing;
    public event EventHandler<ItemActedEventArgs<TValue?>>? Changed;

    private TValue? _Value;
    public TValue? Value
    {
        get => this._Value;
        set
        {
            var e = this.OnChanging(new ItemActingEventArgs<TValue?>(value));
            if (e.Handled)
            {
                return;
            }

            this._Value = value;
            this.OnChanged(new ItemActedEventArgs<TValue?>(this.Value));
        }
    }

    private ItemActingEventArgs<TValue?> OnChanging(ItemActingEventArgs<TValue?> e)
        => e.Fluent(() => Changing?.Invoke(this, e));

    private void OnChanged(ItemActedEventArgs<TValue?> e)
        => Changed?.Invoke(this, e);
}

