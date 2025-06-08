using System.Collections.Immutable;
using System.Data;

using Microsoft.Data.SqlClient;

namespace DataLib;

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
        public async IAsyncEnumerable<(string Schema, string Name)> GetTableNamesAsync(SqlConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            using var cmd = connection.CreateCommand();
            cmd.CommandText = """
                SELECT
                   s.name AS SchemaName,
                    t.name AS TableName
                FROM sys.tables t
                LEFT JOIN sys.schemas s ON t.schema_id = s.schema_id;
                """;

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                yield return (reader.GetString(0), reader.GetString(1));
            }
        }

        public static async Task<ImmutableArray<Table>> GetTablesAsync(this Database database, SqlConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }
            using var cmd = connection.CreateCommand();
            cmd.CommandText = """
                SELECT
                    s.name AS SchemaName,
                    t.name AS TableName
                FROM sys.tables t
                LEFT JOIN sys.schemas s ON t.schema_id = s.schema_id;
                """;
            await using var reader = await cmd.ExecuteReaderAsync();
            var tablesBuilder = ImmutableArray.CreateBuilder<Table>();
            while (await reader.ReadAsync())
            {
                var schemaName = reader.GetString(0);
                var tableName = reader.GetString(1);
                tablesBuilder.Add(new Table { Name = $"{schemaName}.{tableName}" });
            }
            return tablesBuilder.ToImmutable();
        }
    }
}