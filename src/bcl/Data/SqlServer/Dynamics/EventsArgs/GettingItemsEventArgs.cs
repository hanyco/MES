namespace Library.Data.SqlServer.Dynamics.EventsArgs;

public sealed class GettingItemsEventArgs<TSqlObject> : EventArgs
{

    public IEnumerable<TSqlObject> Items { get; set; }

    public GettingItemsEventArgs(IEnumerable<TSqlObject> items) 
        => this.Items = items;
}
