using System.Data;
using System.Runtime.CompilerServices;

using Microsoft.Data.SqlClient;

namespace DataLib.Extensions;

public static class SqlExtension
{
    extension(SqlConnection connection)
    {
        public async IAsyncEnumerable<TResult> ReadAsync<TResult>(string query, Func<SqlDataReader, TResult> getFunc, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var was_open = false;
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
                was_open = true;
            }

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;

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