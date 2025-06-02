using System.ComponentModel;
using System.Data;
using System.Dynamic;

using Microsoft.Data.SqlClient;

namespace Library.Data.SqlServer.Dynamics;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class SqlObject<TSqlObject, TOwner> : DynamicObject, ISqlObject
    where TSqlObject : SqlObject<TSqlObject, TOwner>
{
    protected SqlObject(TOwner owner, string name, string? schema = null, string? connectionString = null)
    {
        this.Owner = owner;
        this.Name = name;
        this.ConnectionString = new SqlConnectionStringBuilder(connectionString).ConnectionString;
        this.Schema = schema;
    }

    public string ConnectionString { get; }

    public virtual string Name { get; }

    public virtual TOwner Owner { get; }

    public string? Schema { get; }

    public override string ToString()
        => this.Schema.IsNullOrEmpty() ? this.Name : $"{this.Schema}.{this.Name}";

    protected static IEnumerable<DataRow> GetDataRows(string connectionString, string tableName)
    {
        using var conn = new SqlConnection(connectionString);
        using var dataSet = conn.FillDataSetByTableNames(tableName);
        return SqlObject<TSqlObject, TOwner>.GetRows(dataSet);
    }

    protected static IEnumerable<DataRow> GetQueryItems(string connectionString, string query)
    {
        using var conn = new SqlConnection(connectionString);
        using var dataSet = conn.FillDataSet(query);
        return GetRows(dataSet);
    }

    protected static Sql GetSql(string connectionString)
        => new(connectionString);

    protected IEnumerable<DataRow> GetDataRows(string query)
        => GetDataRows(this.ConnectionString, query);

    protected IEnumerable<DataRow> GetQueryItems(string query)
        => GetQueryItems(this.ConnectionString, query);

    protected Sql GetSql()
        => GetSql(this.ConnectionString);

    private static IEnumerable<DataRow> GetRows(DataSet ds)
         => ds.Dispose(ds.GetTables().FirstOrDefault()?.Dispose(t => t?.Select())) ?? Enumerable.Empty<DataRow>();
}