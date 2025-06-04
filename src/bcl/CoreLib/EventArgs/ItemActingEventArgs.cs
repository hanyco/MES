namespace Library.EventsArgs;

public sealed class ItemActingEventArgs<TItem> : ActingEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ItemActingEventArgs{TItem}" /> class.
    /// </summary>
    public ItemActingEventArgs() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ItemActingEventArgs{TItem}" /> class.
    /// </summary>
    /// <param name="item">The item.</param>
    public ItemActingEventArgs(in TItem item) => this.Item = item;

    /// <summary>
    ///     Gets or sets the item.
    /// </summary>
    /// <value>
    ///     The item.
    /// </value>
    public TItem Item { get; set; }
}
