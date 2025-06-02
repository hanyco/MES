using System.Data;
using Microsoft.Data.SqlClient;
using System.Dynamic;

using Library.Data.SqlServer.Builders;
using Library.Data.SqlServer.Dynamics.Collections;
using Library.Exceptions;
using Library.Helpers;
using Library.Validations;

namespace Library.Data.SqlServer.Dynamics;

public sealed class Database : SqlObject<Database, Server>
{
    private IEnumerable<DataRow>? _allParams;
    private StoredProcedures? _storedProcedures;
    private Tables? _tables;

    internal Database(in Server server, in string name, in string connectionString, in string? collationName = null) : base(server, name, connectionString: connectionString)
        => this.CollationName = collationName;

    public string? CollationName { get; internal set; }
    public DateTime CreateDate { get; internal set; }
    public long Id { get; internal set; }
    public bool IsBrokerEnabled { get; internal set; }
    public StoredProcedures StoredProcedures => this._storedProcedures ??= this.GetStoredProcedures();
    public Tables Tables => this._tables ??= this.GetTables();
    internal IEnumerable<DataRow> AllParams => this._allParams ??= GetQueryItems(this.ConnectionString, QueryBank.DATABASE_EXTENDED_PROPERTIES);

    public static Database? GetDatabase(in string connectionString, string? name = null)
    {
        Check.MustBeArgumentNotNull(connectionString);
        if (name.IsNullOrEmpty())
        {
            name = new SqlConnectionStringBuilder(connectionString).InitialCatalog;
        }
        Check.MustBeNotNull(name, () => new ObjectNotFoundException("Database"));

        return GetDatabasesCore(connectionString).FirstOrDefault(db => db.Name == name);
    }

    public static async Task<Database?> GetDatabaseAsync(string connectionString, string? name = null, CancellationToken cancellationToken = default)
        => await Task.Run(() => GetDatabase(connectionString, name), cancellationToken);

    public static Databases GetDatabases(string connectionString)
        => GetDatabasesCore(connectionString);

    public static Databases GetDatabases(string datasource, string username, string password)
        => GetDatabasesCore(ConnectionStringBuilder.Build(datasource, username, password));

    public int GetTablesCount()
        => this.GetSql().ExecuteScalarQuery("SELECT COUNT(name) FROM sys.tables")!.Cast().ToInt();

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        result = this.Tables[binder.Name];
        return true;
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        result = this.StoredProcedures[binder.Name]?.Run(args.Compact().Select((t, i) => new KeyValuePair<string, object>(binder.CallInfo.ArgumentNames[i], t)).ToArray());
        return true;
    }

    protected static Databases GetDatabasesCore(string connectionString, string databases = "sys.databases")
        => new(() => GatherDatabases(connectionString, databases));

    private static IEnumerable<Database> GatherDatabases(string connectionString, string databases)
    {
        Check.MustBeArgumentNotNull(connectionString);

        var builder = new SqlConnectionStringBuilder(connectionString);
        var rows = GetDataRows(connectionString, databases);
        if (rows?.Any() is true)
        {
            foreach (var row in rows)
            {
                if (builder.InitialCatalog.IsNullOrEmpty())
                {
                    builder.InitialCatalog = row.Field<string>("name");
                }

                var name = row.Field<string>("name").NotNull();
                yield return new Database(Server.GetInstance(builder.DataSource, connectionString), name, connectionString)
                {
                    CollationName = row.Field<string>("collation_name"),
                    CreateDate = row.Field("create_date", Convert.ToDateTime),
                    Id = row.Field("database_id", Convert.ToInt64),
                    IsBrokerEnabled = row.Field("is_broker_enabled", Convert.ToBoolean)
                };
            }
        }
    }

    private StoredProcedures GetStoredProcedures() => new(
        from row in GetQueryItems(this.ConnectionString, "\r\nexec sp_executesql N'SELECT\r\nsp.name AS [Name],\r\nsp.object_id AS [ID],\r\nsp.create_date AS [CreateDate],\r\nsp.modify_date AS [DateLastModified],\r\nISNULL(ssp.name, N'''') AS [Owner],\r\nCAST(case when sp.principal_id is null then 1 else 0 end AS bit) AS [IsSchemaOwned],\r\nSCHEMA_NAME(sp.schema_id) AS [Schema],\r\nCAST(\r\n case \r\n    when sp.is_ms_shipped = 1 then 1\r\n    when (\r\n        select \r\n            major_id \r\n        from \r\n            sys.extended_properties \r\n        where \r\n            major_id = sp.object_id and \r\n            minor_id = 0 and \r\n            class = 1 and \r\n            name = N''microsoft_database_tools_support'') \r\n        is not null then 1\r\n    else 0\r\nend          \r\n             AS bit) AS [IsSystemObject],\r\nCAST(OBJECTPROPERTYEX(sp.object_id,N''ExecIsAnsiNullsOn'') AS bit) AS [AnsiNullsStatus],\r\nCAST(OBJECTPROPERTYEX(sp.object_id,N''ExecIsQuotedIdentOn'') AS bit) AS [QuotedIdentifierStatus],\r\nCAST(CASE WHEN ISNULL(smsp.definition, ssmsp.definition) IS NULL THEN 1 ELSE 0 END AS bit) AS [IsEncrypted],\r\nCAST(ISNULL(smsp.is_recompiled, ssmsp.is_recompiled) AS bit) AS [Recompile],\r\ncase when amsp.object_id is null then N'''' else asmblsp.name end AS [AssemblyName],\r\ncase when amsp.object_id is null then N'''' else amsp.assembly_class end AS [ClassName],\r\ncase when amsp.object_id is null then N'''' else amsp.assembly_method end AS [MethodName],\r\ncase when amsp.object_id is null then case isnull(smsp.execute_as_principal_id, -1) when -1 then 1 when -2 then 2 else 3 end else case isnull(amsp.execute_as_principal_id, -1) when -1 then 1 when -2 then 2 else 3 end end AS [ExecutionContext],\r\ncase when amsp.object_id is null then ISNULL(user_name(smsp.execute_as_principal_id),N'''') else user_name(amsp.execute_as_principal_id) end AS [ExecutionContextPrincipal],\r\nCAST(ISNULL(spp.is_auto_executed,0) AS bit) AS [Startup],\r\nCAST(CASE sp.type WHEN N''RF'' THEN 1 ELSE 0 END AS bit) AS [ForReplication],\r\nCASE WHEN sp.type = N''P'' THEN 1 WHEN sp.type = N''PC'' THEN 2 ELSE 1 END AS [ImplementationType]\r\nFROM\r\nsys.all_objects AS sp\r\nLEFT OUTER JOIN sys.database_principals AS ssp ON ssp.principal_id = ISNULL(sp.principal_id, (OBJECTPROPERTY(sp.object_id, ''OwnerId'')))\r\nLEFT OUTER JOIN sys.sql_modules AS smsp ON smsp.object_id = sp.object_id\r\nLEFT OUTER JOIN sys.system_sql_modules AS ssmsp ON ssmsp.object_id = sp.object_id\r\nLEFT OUTER JOIN sys.assembly_modules AS amsp ON amsp.object_id = sp.object_id\r\nLEFT OUTER JOIN sys.assemblies AS asmblsp ON asmblsp.assembly_id = amsp.assembly_id\r\nLEFT OUTER JOIN sys.procedures AS spp ON spp.object_id = sp.object_id\r\nWHERE\r\n(sp.type = @_msparam_0 OR sp.type = @_msparam_1 OR sp.type=@_msparam_2)and(SCHEMA_NAME(sp.schema_id)=@_msparam_4)',N'@_msparam_0 nvarchar(4000),@_msparam_1 nvarchar(4000),@_msparam_2 nvarchar(4000),@_msparam_4 nvarchar(4000)',@_msparam_0=N'P',@_msparam_1=N'RF',@_msparam_2=N'PC',@_msparam_4=N'dbo'\r\n")
        select new StoredProcedure(this, "", row.Field<string>("name")!, this.ConnectionString)
        {
            AssemblyName = row.Field("AssemblyName", Convert.ToString)!,
            Id = row.Field("ID", Convert.ToInt64),
            CreateDate = row.Field("CreateDate", Convert.ToDateTime),
            Body = GetQueryItems(this.ConnectionString, $"\r\nexec sp_executesql N'SELECT\r\nNULL AS [Text],\r\nISNULL(smsp.definition, ssmsp.definition) AS [Definition]\r\nFROM\r\nsys.all_objects AS sp\r\nLEFT OUTER JOIN sys.sql_modules AS smsp ON smsp.object_id = sp.object_id\r\nLEFT OUTER JOIN sys.system_sql_modules AS ssmsp ON ssmsp.object_id = sp.object_id\r\nWHERE\r\n(sp.type = @_msparam_0 OR sp.type = @_msparam_1 OR sp.type=@_msparam_2)and(sp.name=@_msparam_3 and SCHEMA_NAME(sp.schema_id)=@_msparam_4)',N'@_msparam_0 nvarchar(4000),@_msparam_1 nvarchar(4000),@_msparam_2 nvarchar(4000),@_msparam_3 nvarchar(4000),@_msparam_4 nvarchar(4000)',@_msparam_0=N'P',@_msparam_1=N'RF',@_msparam_2=N'PC',@_msparam_3=N'{row.Field<string>("name")}',@_msparam_4=N'dbo'\r\n")
                .First()
                .Field("Definition", Convert.ToString)!,
            IsSystemObject = row.Field("IsSystemObject", Convert.ToBoolean),
            LastModifiedDate = row.Field("DateLastModified", Convert.ToDateTime),
            Schema = row.Field("Schema", Convert.ToString)!
        });

    private Tables GetTables()
    {
        var tablesQueryItems = this.GetQueryItems("SELECT name, SCHEMA_NAME(schema_id) AS [schema], create_date, object_id, modify_date FROM sys.tables");
        var tables = tablesQueryItems.Select(r => new Table(this, r.Field<string>("name")!, r.Field<string>("schema"), this.ConnectionString)
        {
            CreateDate = r.Field("create_date", Convert.ToDateTime),
            Id = r.Field("object_id", Convert.ToInt64),
            ModifyDate = r.Field("modify_date", Convert.ToDateTime)
        });
        return new(tables);
    }
}