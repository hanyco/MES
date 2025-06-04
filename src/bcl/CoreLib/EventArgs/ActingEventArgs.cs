namespace Library.EventsArgs;

public class ActingEventArgs : EventArgs
{
    /// <summary>
    ///     Gets or sets a value indicating whether this <see cref="ActingEventArgs" /> is handled.
    /// </summary>
    /// <value>
    ///     <c>true</c> if handled; otherwise, <c>false</c>.
    /// </value>
    public bool Handled { get; set; }
}
