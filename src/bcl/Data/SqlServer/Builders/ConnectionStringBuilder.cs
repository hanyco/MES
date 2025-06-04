using Library.Extensions;
using Library.Resulting;
using Library.Validations;

using Microsoft.Data.SqlClient;

namespace Library.Data.SqlServer.Builders;

/// <summary>
/// Fluent builder for creating SQL Server connection strings.
/// </summary>
public sealed class ConnectionStringBuilder : IValidatable<ConnectionStringBuilder>
{
    private readonly Microsoft.Data.SqlClient.SqlConnectionStringBuilder _builder;

    /// <summary>
    /// Private constructor for creating a ConnectionStringBuilder.
    /// </summary>
    /// <param name="connectionString">Optional initial connection string.</param>
    private ConnectionStringBuilder(string? connectionString = null) =>
        this._builder = connectionString.IsNullOrEmpty() ? new() : new(connectionString);

    /// <summary>
    /// Build a connection string with various optional parameters.
    /// </summary>
    /// <param name="server">The SQL Server instance name or IP address.</param>
    /// <param name="userName">The SQL Server login username.</param>
    /// <param name="password">The SQL Server login password.</param>
    /// <param name="database">The name of the initial database to connect to.</param>
    /// <param name="isIntegratedSecurity">Indicates whether integrated security should be used.</param>
    /// <param name="shouldPersistSecurityInfo">Indicates whether security info should be persisted.</param>
    /// <param name="connectTimeout">The connection timeout value.</param>
    /// <param name="attachDbFilename">The name of the primary database file.</param>
    /// <param name="applicationName">The application name to be used in SQL Server.</param>
    /// <param name="hasMultipleActiveResultSets">
    /// Indicates whether multiple active result sets are allowed.
    /// </param>
    /// <param name="isEncrypt">Indicates whether the connection should be encrypted.</param>
    /// <param name="isUserInstance">Indicates whether to use a user instance of SQL Server.</param>
    /// <param name="isReadOnly">Indicates whether the connection is read-only.</param>
    /// <returns>The constructed connection string.</returns>
    public static string Build(string server,
    string? userName = null,
    string? password = null,
    string? database = null,
    bool? isIntegratedSecurity = null,
    bool? shouldPersistSecurityInfo = null,
    int? connectTimeout = 30,
    string? attachDbFilename = null,
    string? applicationName = null,
    bool? hasMultipleActiveResultSets = null,
    bool? isEncrypt = null,
    bool? isUserInstance = null,
    bool? isReadOnly = null)
    {
        var builder = Create().Server(server);

        // Set properties only if the corresponding values are provided
        if (!string.IsNullOrEmpty(userName))
        {
            builder = builder.UserName(userName);
        }

        if (!string.IsNullOrEmpty(password))
        {
            builder = builder.Password(password);
        }

        if (!string.IsNullOrEmpty(database))
        {
            builder = builder.DataBase(database);
        }

        if (isIntegratedSecurity.HasValue)
        {
            builder = builder.IsIntegratedSecurity(isIntegratedSecurity.Value);
        }

        if (shouldPersistSecurityInfo.HasValue)
        {
            builder = builder.ShouldPersistSecurityInfo(shouldPersistSecurityInfo.Value);
        }

        if (connectTimeout.HasValue)
        {
            builder = builder.ConnectTimeout(connectTimeout.Value);
        }

        if (!string.IsNullOrEmpty(attachDbFilename))
        {
            builder = builder.AttachDbFilename(attachDbFilename);
        }

        if (!string.IsNullOrEmpty(applicationName))
        {
            builder = builder.ApplicationName(applicationName);
        }

        if (hasMultipleActiveResultSets.HasValue)
        {
            builder = builder.MultipleActiveResultSets(hasMultipleActiveResultSets.Value);
        }

        if (isEncrypt.HasValue)
        {
            builder = builder.IsEncrypted(isEncrypt.Value);
        }

        if (isUserInstance.HasValue)
        {
            builder = builder.IsUserInstance(isUserInstance.Value);
        }

        if (isReadOnly.HasValue)
        {
            builder = builder.IsReadOnly(isReadOnly.Value);
        }

        return builder.Build();
    }

    /// <summary>
    /// Create a new ConnectionStringBuilder without an initial connection string.
    /// </summary>
    public static ConnectionStringBuilder Create()
        => new();

    /// <summary>
    /// Create a new ConnectionStringBuilder with a provided initial connection string.
    /// </summary>
    /// <param name="connectionString">The initial connection string.</param>
    public static ConnectionStringBuilder Create(string connectionString)
        => new(connectionString);

    /// <summary>
    /// Validate a connection string.
    /// </summary>
    /// <param name="connectionString">The connection string to validate.</param>
    /// <returns>A Result containing the validated ConnectionStringBuilder.</returns>
    public static Result<ConnectionStringBuilder> Validate(string connectionString)
        => Create(connectionString.ArgumentNotNull()).Validate();

    /// <summary>
    /// Set the ApplicationName in the connection string.
    /// </summary>
    /// <param name="value">The ApplicationName value.</param>
    /// <returns>The updated ConnectionStringBuilder.</returns>
    public ConnectionStringBuilder ApplicationName(string value)
    {
        this._builder.ApplicationName = value;
        return this;
    }

    /// <summary>
    /// Set the AttachDbFilename in the connection string.
    /// </summary>
    /// <param name="value">The AttachDbFilename value.</param>
    /// <returns>The updated ConnectionStringBuilder.</returns>
    public ConnectionStringBuilder AttachDbFilename(string value)
    {
        this._builder.AttachDBFilename = value;
        return this;
    }

    /// <summary>
    /// Build the final connection string.
    /// </summary>
    /// <returns>The constructed connection string.</returns>
    public string Build()
        => this._builder.ConnectionString;

    // Set the ConnectTimeout in the connection string.
    public ConnectionStringBuilder ConnectTimeout(int value)
    {
        this._builder.ConnectTimeout = value;
        return this;
    }

    // Set the DataSource (Server) in the connection string.
    public ConnectionStringBuilder DataBase(string value)
    {
        this._builder.DataSource = value;
        return this;
    }

    // Set the Encrypt option in the connection string.
    public ConnectionStringBuilder IsEncrypted(bool value)
    {
        this._builder.Encrypt = value;
        return this;
    }

    // Set the IntegratedSecurity option in the connection string.
    public ConnectionStringBuilder IsIntegratedSecurity(bool value)
    {
        this._builder.IntegratedSecurity = value;
        return this;
    }

    // Set the ReadOnly or ReadWrite option in the connection string.
    public ConnectionStringBuilder IsReadOnly(bool value)
    {
        this._builder.ApplicationIntent = value ? ApplicationIntent.ReadOnly : ApplicationIntent.ReadWrite;
        return this;
    }

    // Set the UserInstance option in the connection string.
    public ConnectionStringBuilder IsUserInstance(bool value)
    {
        this._builder.UserInstance = value;
        return this;
    }

    // Set the MultipleActiveResultSets option in the connection string.
    public ConnectionStringBuilder MultipleActiveResultSets(bool value)
    {
        this._builder.MultipleActiveResultSets = value;
        return this;
    }

    // Set the Password in the connection string.
    public ConnectionStringBuilder Password(string value)
    {
        this._builder.Password = value;
        return this;
    }

    // Set the Server (DataSource) in the connection string.
    public ConnectionStringBuilder Server(string value)
    {
        this._builder.DataSource = value;
        return this;
    }

    // Set the PersistSecurityInfo option in the connection string.
    public ConnectionStringBuilder ShouldPersistSecurityInfo(bool value)
    {
        this._builder.PersistSecurityInfo = value;
        return this;
    }

    public ConnectionStringBuilder UserName(string value)
    {
        this._builder.UserID = value;
        return this;
    }

    // Validate the connection string.
    public Result<ConnectionStringBuilder> Validate()
        => new(this);

    IResult<ConnectionStringBuilder> IValidatable<ConnectionStringBuilder>.Validate() => this.Validate();

}