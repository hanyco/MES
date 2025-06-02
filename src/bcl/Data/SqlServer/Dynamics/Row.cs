using System.Dynamic;
using Library.Exceptions;

namespace Library.Data.SqlServer.Dynamics;

public sealed class Row : SqlObject<Row, Table>
{
    public Row(Table owner, IEnumerable<KeyValuePair<string, object?>> data, string? connectionString = null)
        : base(owner, string.Empty, connectionString: connectionString ?? owner.ConnectionString) => this.Data = data;

    public object this[Column index] => this[index.Name];

    public object this[string colName]
    {
        get
        {
            var q = from pair in this.Data
                    where pair.Key.EqualsTo(colName)
                    select pair;
            return !q.Any() ? throw new ObjectNotFoundException(colName) : q.First().Value;
        }
    }

    public object this[int index] => this.Data.ElementAt(index);

    private IEnumerable<KeyValuePair<string, object>> Data { get; }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        result = this[binder.Name];
        return true;
    }
}
