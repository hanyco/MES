using System.Data;
using System.Runtime.CompilerServices;

using DataLib;
using DataLib.Extensions;

using Microsoft.Data.SqlClient;

namespace DataLib.Extensions;

public static class MetadataExtension
{
    extension(Database)
    {
        /// <summary>
        /// Asynchronously retrieves the fully qualified names of all tables in the specified SQL database.
        /// </summary>
        /// <remarks>
        /// This method uses an asynchronous enumerable to return table names one at a time as they
        /// are read from the database. Ensure that the <paramref name="connection" /> is properly
        /// configured to access the target database. The caller is responsible for handling any
        /// exceptions that may occur during database access, such as connection errors or SQL
        /// execution issues.
        /// </remarks>
        /// <param name="connection">
        /// The <see cref="SqlConnection" /> instance used to connect to the database. The
        /// connection must be valid and can be in any state. If the connection is not open, it will
        /// be opened automatically.
        /// </param>
        /// <returns>
        /// An asynchronous stream of strings, where each string represents the fully qualified name
        /// of a table in the format "schema.table".
        /// </returns>
        public async IAsyncEnumerable<(int ObjectId, string Schema, string Name)> GetTables(SqlConnection connection, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                	t.object_id,
                    s.name AS [schema],
                    t.name AS [name]
                FROM sys.tables t
                LEFT JOIN sys.schemas s ON t.schema_id = s.schema_id;
                """;
            var resultTask = connection.ReadAsync(sql,
                static reader => (reader.GetInt32("object_id"), reader.GetString("schema"), reader.GetString("name")),
                cancellationToken);
            await foreach (var item in resultTask)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
            }
        }

        public async IAsyncEnumerable<(int ObjectId, string Schema, string Name)> GetTables(string connectionString, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                	t.object_id,
                    s.name AS [schema],
                    t.name AS [name]
                FROM sys.tables t
                LEFT JOIN sys.schemas s ON t.schema_id = s.schema_id;
                """;
            var resultTask = SqlConnection.ReadAsync(
                connectionString, sql,
                static reader => (reader.GetInt32("object_id"), reader.GetString("schema"), reader.GetString("name")),
                cancellationToken);
            await foreach (var item in resultTask)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
            }
        }

        //public async Task<ImmutableArray<Table>> GetTables(SqlConnection connection)
        //{
        //    if (connection.State != ConnectionState.Open)
        //    {
        //        await connection.OpenAsync();
        //    }
        //    using var cmd = connection.CreateCommand();
        //    cmd.CommandText = """
        //        SELECT
        //        	t.object_id,
        //            s.name AS [schema],
        //            t.name AS [name]
        //        FROM sys.tables t
        //        LEFT JOIN sys.schemas s ON t.schema_id = s.schema_id;

        //        SELECT t.object_id, t.name [table_name], c.name [column_name], c.column_id, [type].name type_name, c.is_identity, c.is_nullable FROM sys.tables t
        //        LEFT JOIN sys.columns c ON c.object_id = t.object_id
        //        LEFT JOIN sys.types [type] ON [type].system_type_id = c.system_type_id;
        //        """;
        //    await using var reader = await cmd.ExecuteReaderAsync();
        //    var tablesBuilder = ImmutableArray.CreateBuilder<Table>();
        //    while (await reader.ReadAsync())
        //    {
        //        var id = reader.GetInt32("object_id");
        //        var schemaName = reader.GetString("schema");
        //        var tableName = reader.GetString("name");
        //        tablesBuilder.Add(new Table { ObjectId = id, Name = tableName, Schema = schemaName });
        //    }
        //    return tablesBuilder.ToImmutable();
        //}
    }
}