using System.Data;
using Microsoft.Data.SqlClient;
using System.Dynamic;
using Library.Data.SqlClient;
using Library.Data.SqlServer.Dynamics.Collections;

namespace Library.Data.SqlServer.Dynamics;

public sealed class Server : DynamicObject, ISqlObject
{
    private Databases _Databases;

    protected SqlConnectionStringBuilder ConnectionStringBuilder { get; } = new SqlConnectionStringBuilder();

    protected Server()
    {
    }

    /// <summary>
    ///     Gets or sets the connection string.
    /// </summary>
    /// <value>The connection string.</value>
    public string ConnectionString
    {
        get
        {
            this.ConnectionStringBuilder.DataSource = this.Name;
            return this.ConnectionStringBuilder.ConnectionString;
        }
        set
        {
            this.ConnectionStringBuilder.ConnectionString = value;
            if (!this.ConnectionStringBuilder.DataSource.IsNullOrEmpty())
            {
                this.Name = this.ConnectionStringBuilder.DataSource;
            }
            else
            {
                this.ConnectionStringBuilder.DataSource = this.Name;
            }
        }
    }

    public Databases Databases => this._Databases ??= this.GetDatabases();

    public string Name { get; protected set; }

    public static Servers Servers => new(from instance in SqlBrowserClient.Instances
                                         select new Server
                                         {
                                             Name = instance.InstanceName.IsNullOrEmpty()
                                                 ? instance.ServerName
                                                 : $@"{instance.ServerName}\{instance.InstanceName}",
                                             Version = instance.Version
                                         });

    public string Version { get; protected set; }

    public static Server GetServer(string connectionString) => new() { ConnectionString = connectionString };

    internal static Server GetInstance() => new();

    internal static Server GetInstance(string name) => new() { Name = name };

    internal static Server GetInstance(string name, string connectionString) => new()
    {
        Name = name,
        ConnectionString = connectionString
    };

    public override string ToString() => !this.Version.IsNullOrEmpty() ? $"{this.Name} - {this.Version}" : $"{this.Name}";

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        result = this.Databases[binder.Name];
        return true;
    }

    internal void SetName(string name) => this.Name = name;

    protected Databases GetDatabases()
    {
        using var conn = new SqlConnection(this.ConnectionString);
        var items = new List<Database>();
        using (var set = conn.FillDataSetByTableNames("sys.databases"))
        {
            var table = set.GetTables().First();
            Database selector(DataRow row) => new(this, row.Field<string>("name"), this.ConnectionString)
            {
                CollationName = row
                    .Field<string>(
                        "collation_name"),
                CreateDate = row.Field(
                    "create_date",
                    Convert.ToDateTime),
                Id = row.Field("database_id",
                    Convert.ToInt64),
                IsBrokerEnabled =
                    row.Field(
                        "is_broker_enabled",
                        Convert.ToBoolean)
            };
            items.AddRange(table.Select().Select(selector));
        }

        return new Databases(items);
    }
}
