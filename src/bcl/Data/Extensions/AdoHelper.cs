using System.Data;
using System.Globalization;

using Library.Data.SqlServer;
using Library.Results;
using Library.Validations;

using Microsoft.Data.SqlClient;

namespace Library.Helpers;

/// <summary>
/// A utility to do some common tasks about ADO arguments
/// </summary>
public static class AdoHelper
{
    /// <summary>
    /// Checks if a SqlConnection can connect.
    /// </summary>
    /// <param name="conn">The SqlConnection to check.</param>
    /// <returns>True if the SqlConnection can connect, false otherwise.</returns>
    public static async Task<bool> CanConnectAsync([DisallowNull] this SqlConnection conn, CancellationToken cancellationToken = default)
    {
        var result = await conn.TryConnectAsync(cancellationToken: cancellationToken);
        return result.IsSucceed;
    }

    /// <summary>
    /// Creates a SQL command associated with a given connection and command text.
    /// </summary>
    /// <param name="connection">The database connection used to create the SQL command.</param>
    /// <param name="commandText">The SQL command text that will be executed.</param>
    /// <param name="fillParams">An optional action to populate the command's parameters.</param>
    /// <returns>Returns the created SqlCommand object.</returns>
    public static SqlCommand CreateCommand([DisallowNull] this SqlConnection connection, [DisallowNull] string commandText, Action<SqlParameterCollection>? fillParams = null)
    {
        Check.MustBeArgumentNotNull(connection);
        Check.MustBeArgumentNotNull(commandText);

        var result = connection.CreateCommand();
        result.CommandText = commandText;
        result.CommandTimeout = connection.ConnectionTimeout;
        fillParams?.Invoke(result.Parameters);
        return result;
    }

    /// <summary>
    /// Creates a SQL command using the provided connection and SQL query string.
    /// </summary>
    /// <param name="conn">The database connection used to create the SQL command.</param>
    /// <param name="sql">The SQL query string that will be set as the command text.</param>
    /// <returns>Returns a SqlCommand object configured with the specified connection and command text.</returns>
    public static SqlCommand CreateCommand([DisallowNull] this SqlConnection conn, [DisallowNull] string sql)
    {
        Check.MustBeArgumentNotNull(conn);
        Check.MustBeArgumentNotNull(sql);

        var result = conn.CreateCommand();
        result.CommandText = sql;
        return result;
    }

    public static void EnsureClosed([DisallowNull] this SqlConnection connection, [DisallowNull] Action<SqlConnection> action, bool openConnection = false)
    {
        Check.MustBeArgumentNotNull(action);

        _ = connection.EnsureClosed(c =>
        {
            action(c);
            return true;
        },
        openConnection);
    }

    public static void EnsureClosed([DisallowNull] this SqlConnection connection, [DisallowNull] Action action, bool openConnection = false)
    {
        Check.MustBeArgumentNotNull(action);

        _ = connection.EnsureClosed(_ =>
                {
                    action();
                    return true;
                },
                openConnection);
    }

    /// <summary>
    /// Ensures that a SQL connection is closed after executing a specified action. It optionally opens the connection
    /// before execution.
    /// </summary>
    /// <typeparam name="TResult">This type parameter represents the result type returned by the specified action.</typeparam>
    /// <param name="connection">This parameter represents the SQL connection to be managed during the execution of the action.</param>
    /// <param name="action">This parameter is a function that defines the action to be executed while managing the connection.</param>
    /// <param name="openConnection">This parameter indicates whether to open the connection before executing the action.</param>
    /// <param name="cancellationToken">This parameter allows for the operation to be canceled if needed.</param>
    /// <returns>The return value is the result produced by executing the specified action.</returns>
    public static TResult EnsureClosed<TResult>([DisallowNull] this SqlConnection connection, [DisallowNull] Func<TResult> action, bool openConnection = false, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(action);
        return connection.EnsureClosed(_ => action(), openConnection);
    }
    /// <summary>
    /// Ensures that a SQL connection is closed after executing a specified action. It can optionally open the
    /// connection before execution.
    /// </summary>
    /// <typeparam name="TResult">This type parameter represents the result type returned by the specified action.</typeparam>
    /// <param name="connection">This parameter represents the SQL connection that will be managed during the execution of the action.</param>
    /// <param name="action">This parameter is a function that will be executed while ensuring the connection is properly closed afterward.</param>
    /// <param name="openConnection">This parameter indicates whether to open the connection before executing the action.</param>
    /// <returns>The result of executing the specified action.</returns>
    public static TResult EnsureClosed<TResult>([DisallowNull] this SqlConnection connection, [DisallowNull] Func<TResult> action, bool openConnection = false)
    {
        Check.MustBeArgumentNotNull(action);

        return connection.EnsureClosed(_ => action(), openConnection);
    }

    /// <summary>
    /// Ensures a SQL connection is closed after executing a specified action, optionally opening the connection first.
    /// </summary>
    /// <typeparam name="TResult">This type parameter represents the result produced by executing the specified action on the SQL connection.</typeparam>
    /// <param name="connection">This parameter is the SQL connection that will be used to execute the action.</param>
    /// <param name="action">This parameter is a function that takes a SQL connection and returns a result after performing some operations.</param>
    /// <param name="openConnection">This parameter indicates whether to open the SQL connection before executing the action.</param>
    /// <returns>The result produced by the action executed on the SQL connection.</returns>
    public static TResult EnsureClosed<TResult>([DisallowNull] this SqlConnection connection, [DisallowNull] Func<SqlConnection, TResult> action, bool openConnection = false)
    {
        Check.MustBeArgumentNotNull(connection);
        Check.MustBeArgumentNotNull(action);

        try
        {
            if (openConnection)
            {
                connection.Open();
            }

            return action(connection);
        }
        finally
        {
            if (connection?.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }

    public static async Task<TResult> EnsureClosedAsync<TResult>([DisallowNull] this SqlConnection connection,
            [DisallowNull] Func<SqlConnection, Task<TResult>> actionAsync,
            bool openConnection = false, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(connection);
        Check.MustBeArgumentNotNull(actionAsync);

        try
        {
            if (openConnection && connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            return await actionAsync(connection);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public static async Task EnsureClosedAsync([DisallowNull] this SqlConnection connection,
            [DisallowNull] Func<SqlConnection, CancellationToken, Task> actionAsync,
            bool openConnection = false, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(connection);
        Check.MustBeArgumentNotNull(actionAsync);

        try
        {
            if (openConnection && connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await actionAsync(connection, cancellationToken);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public static int ExecuteNonQuery([DisallowNull] this SqlConnection connection, [DisallowNull] string sql, Action<SqlParameterCollection>? fillParams = null)
        => connection.ArgumentNotNull().Execute(cmd => cmd.ExecuteNonQuery(), sql.ArgumentNotNull(), fillParams);

    /// <summary>
    /// Executes a SQL command and returns a SqlDataReader for reading the results.
    /// </summary>
    /// <param name="connection">Establishes the database connection required to execute the SQL command.</param>
    /// <param name="sql">Contains the SQL query to be executed against the database.</param>
    /// <param name="fillParams">Allows for additional parameters to be added to the SQL command before execution.</param>
    /// <param name="behavior">Specifies how the command should be executed and how the results should be processed.</param>
    /// <returns>Provides a SqlDataReader object to read the results of the executed SQL command.</returns>
    public static SqlDataReader ExecuteReader([DisallowNull] this SqlConnection connection,
            [DisallowNull] string sql,
            Action<SqlParameterCollection>? fillParams = null,
            CommandBehavior behavior = CommandBehavior.Default)
    {
        using var command = connection.CreateCommand(sql, fillParams);
        connection.StateChange += (_, e) =>
        {
            if (e.CurrentState == ConnectionState.Closed)
            {
                command?.Dispose();
            }
        };
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        return command.ExecuteReader(behavior);
    }

    public static async Task<SqlDataReader> ExecuteReaderAsync([DisallowNull] this SqlConnection connection,
            [DisallowNull] string sql,
            Action<SqlParameterCollection>? fillParams = null,
            CommandBehavior behavior = CommandBehavior.Default, CancellationToken cancellationToken = default)
    {
        await using var command = connection.CreateCommand(sql, fillParams);
        connection.StateChange += (_, e) =>
        {
            if (e.CurrentState == ConnectionState.Closed)
            {
                command?.Dispose();
            }
        };
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        return await command.ExecuteReaderAsync(behavior, cancellationToken);
    }

    public static object ExecuteScalar([DisallowNull] this SqlConnection connection, [DisallowNull] string sql, Action<SqlParameterCollection>? fillParams = null)
        => connection.Execute(cmd => cmd.ExecuteScalar(), sql, fillParams);

    public static async Task<object?> ExecuteScalarAsync([DisallowNull] this SqlConnection connection, [DisallowNull] string sql, Action<SqlParameterCollection>? fillParams = null, CancellationToken cancellationToken = default)
        => await connection.ExecuteAsync(cmd => cmd.ExecuteScalarAsync(), sql, fillParams, cancellationToken: cancellationToken);

    public static object? ExecuteStoredProcedure([DisallowNull] this SqlConnection connection,
            [DisallowNull] string spName,
            Action<SqlParameterCollection>? fillParams = null,
            Action<string>? logger = null)
    {
        Check.MustBeArgumentNotNull(connection);

        object? result = null;
        using (var cmd = connection.CreateCommand())
        {
            var cmdText = new StringBuilder($"Exec [{spName}]");
            if (fillParams is not null)
            {
                fillParams(cmd.Parameters);
                for (var index = 0; index < cmd.Parameters.Count; index++)
                {
                    var parameter = cmd.Parameters[index];
                    _ = cmdText.Append(CultureInfo.InvariantCulture, $"\t{Environment.NewLine}{parameter.ParameterName} = '{parameter.Value}'");
                    if (index != cmd.Parameters.Count - 1)
                    {
                        _ = cmdText.Append(", ");
                    }
                }
            }

            logger?.Invoke(cmdText.ToString());
            ExecuteTransactional(connection,
                trans =>
                {
                    cmd.Transaction = trans;
                    cmd.CommandText = cmdText.ToString();
                    result = cmd.ExecuteScalar();
                });
        }

        return result;
    }

    public static void ExecuteTransactional([DisallowNull] this SqlConnection connection, [DisallowNull] Action<SqlTransaction>? executionBlock)
    {
        Check.MustBeArgumentNotNull(connection);
        Check.MustBeArgumentNotNull(executionBlock);

        var leaveOpen = connection.State == ConnectionState.Open;
        if (!leaveOpen)
        {
            connection.Open();
        }

        using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
        try
        {
            executionBlock(transaction);
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        finally
        {
            if (!leaveOpen)
            {
                connection.Close();
            }
        }
    }

    /// <summary>
    /// Retrieves a value from a data reader based on the specified column name and converts it to a specified type.
    /// </summary>
    /// <typeparam name="T">Specifies the type to which the retrieved value will be converted.</typeparam>
    /// <param name="reader">Represents the data reader from which the value is being retrieved.</param>
    /// <param name="columnName">Indicates the name of the column from which to fetch the value.</param>
    /// <param name="converter">Defines the method used to convert the retrieved value to the specified type.</param>
    /// <returns>Returns the converted value from the specified column.</returns>
    public static T Field<T>([DisallowNull] this IDataReader reader, [DisallowNull] string columnName, [DisallowNull] Converter<object, T> converter)
    {
        Check.MustBeArgumentNotNull(columnName);
        Check.MustBeArgumentNotNull(converter);
        Check.MustBeArgumentNotNull(reader);

        return converter(reader[columnName]);
    }

    /// <summary>
    /// Retrieves a value from a specified column in a DataRow and converts it to a specified type if a converter is
    /// provided.
    /// </summary>
    /// <typeparam name="T">Specifies the target type to which the value from the DataRow will be converted.</typeparam>
    /// <param name="row">Represents the DataRow from which the value will be retrieved.</param>
    /// <param name="columnName">Indicates the name of the column from which the value is extracted.</param>
    /// <param name="converter">An optional function that converts the value to the specified target type.</param>
    /// <returns>Returns the value from the DataRow as the specified type, or null if the conversion is not possible.</returns>
    public static T? Field<T>([DisallowNull] this DataRow row, [DisallowNull] string columnName, [DisallowNull] Converter<object?, T?>? converter)
    {
        Check.MustBeArgumentNotNull(row);
        Check.MustBeArgumentNotNull(columnName);
        return converter is not null ? converter(row.Field<object>(columnName)) : row.Field<T>(columnName);
    }

    public static DataSet FillDataSet(this SqlConnection connection, string sql)
    {
        Check.MustBeArgumentNotNull(sql);

        var result = new DataSet();
        using (var da = new SqlDataAdapter(sql, connection))
        {
            _ = da.Fill(result);
        }

        return result;
    }

    public static DataSet FillDataSetByTableNames([DisallowNull] this SqlConnection connection, params string[] tableNames)
    {
        Check.MustBeArgumentNotNull(connection);
        Check.MustBeArgumentNotNull(tableNames);

        var result = connection.FillDataSet(tableNames.Select(t => SqlStatementBuilder.CreateSelect(t)).Merge(Environment.NewLine));
        for (var i = 0; i < tableNames.Length; i++)
        {
            result.Tables[i].TableName = tableNames[i];
        }

        return result;
    }

    public static DataTable FillDataTable([DisallowNull] this SqlConnection connection, [DisallowNull] string sql, Action<SqlParameterCollection>? fillParams = null)
    {
        var result = new DataTable();
        using (var command = connection.CreateCommand(sql, fillParams))
        {
            EnsureClosed(connection, () => result.Load(command.ExecuteReader()), true);
        }

        return result;
    }

    public static IEnumerable<DataTable> FillDataTables([DisallowNull] this SqlConnection conn, params string[] queries)
    {
        Check.MustBeArgumentNotNull(queries);

        using var cmd = conn.CreateCommand();
        using var da = new SqlDataAdapter(cmd);

        conn.Open();
        foreach (var query in queries)
        {
            cmd.CommandText = query;
            var dataTable = new DataTable();
            _ = da.Fill(dataTable);
            yield return dataTable;
        }
    }

    public static IEnumerable<DataTable> GetTables(this DataSet ds)
            => ds is null
            ? throw new ArgumentNullException(nameof(ds))
            : ds.Tables.Cast<DataTable>();

    public static DataTable LoadDataTable([DisallowNull] this SqlConnection connection, [DisallowNull] string query, Action<SqlParameterCollection>? fillParams = null)
    {
        var result = new DataTable();
        using var command = connection.CreateCommand(query);
        fillParams?.Invoke(command.Parameters);
        command.Connection.Open();
        result.Load(command.ExecuteReader());
        command.Connection.Close();

        return result;
    }

    public static async Task<DataTable> LoadDataTableAsync([DisallowNull] this SqlConnection conn, [DisallowNull] string query, Action<SqlParameterCollection>? fillParams = null, CancellationToken cancellationToken = default)
    {
        var result = new DataTable();
        await using var command = conn.CreateCommand(query);
        fillParams?.Invoke(command.Parameters);
        await command.Connection.OpenAsync(cancellationToken);
        result.Load(await command.ExecuteReaderAsync(cancellationToken));
        return result;
    }

    /// <summary>
    /// Tries to connect to a SqlConnection asynchronously.
    /// </summary>
    /// <param name="conn">The SqlConnection to connect to.</param>
    /// <returns>An exception if the connection fails, otherwise null.</returns>
    public static async Task<TryMethodResult> TryConnectAsync([DisallowNull] this SqlConnection conn, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(conn);

        try
        {
            await conn.EnsureClosedAsync((c, t) => c.OpenAsync(t), cancellationToken: cancellationToken);
            return TryMethodResult.Success();
        }
        catch (Exception ex)
        {
            return TryMethodResult.Fail(ex);
        }
    }

    private static TResult Execute<TResult>([DisallowNull] this SqlConnection connection,
        [DisallowNull] Func<SqlCommand, TResult> execute,
        [DisallowNull] string sql,
        Action<SqlParameterCollection>? fillParams = null)
    {
        Check.MustBeArgumentNotNull(connection);
        Check.MustBeArgumentNotNull(execute);
        Check.MustBeArgumentNotNull(sql);

        using var command = connection.CreateCommand(sql, fillParams);
        return connection.EnsureClosed(_ => execute(command), true);
    }

    private static async Task<TResult> ExecuteAsync<TResult>([DisallowNull] this SqlConnection connection,
            [DisallowNull] Func<SqlCommand, Task<TResult>> executeAsync,
            [DisallowNull] string sql,
            Action<SqlParameterCollection>? fillParams = null, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(connection);
        Check.MustBeArgumentNotNull(executeAsync);
        Check.MustBeArgumentNotNull(sql);

        await using var command = connection.CreateCommand(sql, fillParams);
        return await connection.EnsureClosedAsync(_ => executeAsync(command), true, cancellationToken: cancellationToken);
    }
}