namespace Library.EventsArgs;

public class ItemActedEventArgs<TItem> : EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ItemActedEventArgs{TItem}" /> class.
    /// </summary>
    /// <param name="item">The item.</param>
    public ItemActedEventArgs(in TItem item) => this.Item = item;

    /// <summary>
    ///     Gets or sets the item.
    /// </summary>
    /// <value>
    ///     The item.
    /// </value>
    public TItem Item { get; init; }
}

public sealed class InitialItemEventArgs<TItem> : EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="InitialItemEventArgs{TItem}" /> class.
    /// </summary>
    /// <param name="item">The item.</param>
    public InitialItemEventArgs()
    { }

    /// <summary>
    ///     Gets or sets the item.
    /// </summary>
    /// <value>
    ///     The item.
    /// </value>
    public TItem Item { get; set; }
}
