using Microsoft.Data.SqlClient;

namespace Library.Data.SqlServer.Dynamics.Collections;

public sealed class Servers : SqlObjects<Server>
{
    internal Servers(IEnumerable<Server> items)
        : base(items)
    {
    }

    public Server? GetCurrent()
    {
        if (!this.Items.Any())
        {
            return null;
        }

        var builder = new SqlConnectionStringBuilder(this[0].ConnectionString);
        return base[builder.DataSource];
    }
}
