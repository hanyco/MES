using System.Data;
using System.Runtime.CompilerServices;

using Library.Extensions;

using Microsoft.Data.SqlClient;

namespace DataLib.Extensions;

public static class AdoExtension
{
    extension(SqlConnection @this)
    {
        public async IAsyncEnumerable<TResult> ReadAsync<TResult>(string sql, Func<SqlDataReader, TResult> getFunc, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var was_open = false;
            if (@this.State != ConnectionState.Open)
            {
                await @this.OpenAsync(cancellationToken);
                was_open = true;
            }

            using var cmd = @this.CreateCommand();
            cmd.CommandText = sql;

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                yield return getFunc(reader);
            }
            if (was_open)
            {
                await @this.CloseAsync();
            }
        }

        public SqlCommand CreateCommand(string commandText)
        {
            var cmd = @this.CreateCommand();
            cmd.CommandText = commandText;
            return cmd;
        }

        public async Task<DataTable> LoadTable(string sql, CancellationToken cancellationToken)
        {
            using var cmd = @this.CreateCommand(sql);
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

    public static async Task ExecuteInTransaction(this SqlConnection @this, Func<Task> action, CancellationToken cancellationToken)
    {
        using var trans = @this.BeginTransaction();
        try
        {
            await action().ThrowIfCancellationRequested(cancellationToken);
            trans.Commit();
        }
        catch
        {
            trans.Rollback();
            throw;
        }
    }

    public static async Task ExecuteInTransaction(this SqlConnection @this, Func<CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        using var trans = @this.BeginTransaction();
        try
        {
            await action(cancellationToken).ThrowIfCancellationRequested(cancellationToken);
            trans.Commit();
        }
        catch
        {
            trans.Rollback();
            throw;
        }
    }

    public static async Task<TResult> ExecuteInTransaction<TResult>(this SqlConnection @this, Func<Task<TResult>> action, CancellationToken ct)
    {
        using var trans = @this.BeginTransaction();
        try
        {
            var result = await action().ThrowIfCancellationRequested(ct);
            trans.Commit();
            return result;
        }
        catch
        {
            trans.Rollback();
            throw;
        }
    }

    public static async Task<TResult> ExecuteInTransaction<TResult>(this SqlConnection @this, Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken)
    {
        using var trans = @this.BeginTransaction();
        try
        {
            var result = await action(cancellationToken).ThrowIfCancellationRequested(cancellationToken);
            trans.Commit();
            return result;
        }
        catch
        {
            trans.Rollback();
            throw;
        }
    }
}