using System.Dynamic;

using Library.Validations;

using Microsoft.Data.SqlClient;

namespace Library.Data.SqlServer.Dynamics;

public sealed class DataReader(SqlDataReader sqlDataReader, Database owner, string name, string connectionString) : SqlObject<DataReader, Database>(owner, name, connectionString: connectionString), IDisposable
{
    public object this[string index] => this.SqlDataReader[index];

    private SqlDataReader SqlDataReader { get; } = sqlDataReader ?? throw new ArgumentNullException(nameof(sqlDataReader));

    public void Dispose()
    {
        this.SqlDataReader.Close();
        this.SqlDataReader.Dispose();
    }

    public bool Read()
        => this.SqlDataReader.Read();
}