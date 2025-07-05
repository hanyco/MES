using System.Data;
using System.Runtime.CompilerServices;

using Microsoft.Data.SqlClient;

namespace DataLib.Extensions;

public static class SqlExtension
{
    extension(SqlConnection connection)
    {
        public async IAsyncEnumerable<TResult> ReadAsync<TResult>(string sql, Func<SqlDataReader, TResult> getFunc, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var was_open = false;
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
                was_open = true;
            }

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                yield return getFunc(reader);
            }
            if (was_open)
            {
                await connection.CloseAsync();
            }
        }

        public SqlCommand CreateCommand(string commandText)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = commandText;
            return cmd;
        }

        public async Task<DataTable> LoadTable(string sql, CancellationToken cancellationToken)
        {
            using var cmd = connection.CreateCommand(sql);
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            var table = new DataTable();
            table.Load(reader);
            return table;
        }
    }

    extension(SqlConnection)
    {
        public static async IAsyncEnumerable<TResult> ReadAsync<TResult>(string connectionString, string query, Func<SqlDataReader, TResult> getFunc, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var conn = new SqlConnection(connectionString);
            var resultTask = conn.ReadAsync(query, getFunc, cancellationToken);
            await foreach (var item in resultTask)
            {
                yield return item;
            }
        }
    }
}