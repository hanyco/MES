namespace Library.Data.SqlServer.Dynamics;

internal static class QueryBank
{
    internal const string COLUMNS_WHERE_TABLE_NAME =
        @"SELECT *, system_type= (SELECT TOP 1 name FROM sys.types WHERE system_type_id = cols.system_type_id) FROM sys.columns AS cols WHERE OBJECT_ID = (SELECT object_id FROM sys.tables WHERE name = '{0}') AND is_hidden = 0";

    internal const string DATABASE_EXTENDED_PROPERTIES =
        "\r\nexec sp_executesql N'SELECT\nsp.name AS [SpName],\nparam.name AS [Name],\nparam.parameter_id AS [ID],\nparam.default_value AS [DefaultValue],\nusrt.name AS [DataType],\nsparam.name AS [DataTypeSchema],\nISNULL(baset.name, N'''') AS [SystemType],\nCAST(CASE WHEN baset.name IN (N''nchar'', N''nvarchar'') AND param.max_length <> -1 THEN param.max_length/2 ELSE param.max_length END AS int) AS [Length],\nCAST(param.precision AS int) AS [NumericPrecision],\nCAST(param.scale AS int) AS [NumericScale],\nISNULL(xscparam.name, N'''') AS [XmlSchemaNamespace],\nISNULL(s2param.name, N'''') AS [XmlSchemaNamespaceSchema],\nISNULL( (case param.is_xml_document when 1 then 2 else 1 end), 0) AS [XmlDocumentConstraint],\nparam.is_output AS [IsOutputParameter],\nparam.is_cursor_ref AS [IsCursorParameter],\nsp.object_id AS [IDText],\ndb_name() AS [DatabaseName],\nparam.name AS [ParamName],\nCAST(\r\n case \r\n    when sp.is_ms_shipped = 1 then 1\r\n    when (\r\n        select \r\n            major_id \r\n        from \r\n            sys.extended_properties \r\n        where \r\n            major_id = sp.object_id and \r\n            minor_id = 0 and \r\n            class = 1 and \r\n            name = N''microsoft_database_tools_support'') \r\n        is not null then 1\r\n    else 0\r\nend          \r\n             AS bit) AS [ParentSysObj],\n1 AS [Number]\nFROM\nsys.all_objects AS sp\nINNER JOIN sys.all_parameters AS param ON param.object_id=sp.object_id\nLEFT OUTER JOIN sys.types AS usrt ON usrt.user_type_id = param.user_type_id\nLEFT OUTER JOIN sys.schemas AS sparam ON sparam.schema_id = usrt.schema_id\nLEFT OUTER JOIN sys.types AS baset ON (baset.user_type_id = param.system_type_id and baset.user_type_id = baset.system_type_id) \nLEFT OUTER JOIN sys.xml_schema_collections AS xscparam ON xscparam.xml_collection_id = param.xml_collection_id\nLEFT OUTER JOIN sys.schemas AS s2param ON s2param.schema_id = xscparam.schema_id\nWHERE\n((sp.type = @_msparam_1 OR sp.type = @_msparam_2 OR sp.type=@_msparam_3)and(SCHEMA_NAME(sp.schema_id)=@_msparam_5))',N'@_msparam_1 nvarchar(4000),@_msparam_2 nvarchar(4000),@_msparam_3 nvarchar(4000),@_msparam_5 nvarchar(4000)',@_msparam_1=N'P',@_msparam_2=N'RF',@_msparam_3=N'PC',@_msparam_5=N'dbo'";

    internal const string FOREIGN_KEY_COLUMNS_WHERE_TABLE_NAME = @"SELECT        (SELECT        name
                          FROM            sys.tables
                          WHERE        (object_id = fk.parent_object_id)) AS ParentTable,
                             (SELECT        name
                               FROM            sys.columns
                               WHERE        (object_id = fk.parent_object_id) AND (column_id = fk.parent_column_id)) AS ParentColumn,
                             (SELECT        name
                               FROM            sys.tables
                               WHERE        (object_id = fk.referenced_object_id)) AS ReferencedTable,
                             (SELECT        name
                               FROM            sys.columns
                               WHERE        (object_id = fk.referenced_object_id) AND (column_id = fk.referenced_column_id)) AS ReferencedColumn
FROM            sys.foreign_key_columns AS fk
WHERE        ((SELECT        name
                            FROM            sys.tables
                            WHERE        (object_id = fk.parent_object_id)) = '{0}');";

    internal const string IDENTITIES_WHERE_TABLE_NAME = @"SELECT c.name AS column_name
						FROM sys.tables AS t
						JOIN sys.identity_columns c ON t.object_id = c.object_id
						WHERE t.name = '{0}';";
}
