using Library.CodeGenLib.Back;
using Library.CodeGenLib.CodeGenerators;
using Library.CodeGenLib.Models;
using Library.Coding;
using Library.Data.SqlServer.Dynamics;

namespace CodeGenerator.Application.Services;

public class DtoService
{
    private readonly string _connectionString;

    public DtoService(string connectionString)
        => _connectionString = connectionString;

    public IEnumerable<string> GetTables()
        => Table.GetByConnectionString(_connectionString)?.Select(t => t.Name)
            ?? Enumerable.Empty<string>();

    public List<FieldDefinition> GetColumns(string tableName)
    {
        var db = Database.GetDatabase(_connectionString);
        if (db is null)
        {
            return [];
        }

        var table = db.Tables.FirstOrDefault(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
        if (table is null)
        {
            return [];
        }

        var columns = Table.GetTableColumns(table);
        return columns.Select(c => new FieldDefinition
            {
                Name = c.Name,
                Type = MapToCSharpType(c.DataType)
            })
            .ToList();
    }

    private static string MapToCSharpType(string sqlType)
        => sqlType.ToLowerInvariant() switch
        {
            "bigint" => "long",
            "int" => "int",
            "smallint" => "short",
            "tinyint" => "byte",
            "bit" => "bool",
            "float" => "double",
            "real" => "float",
            "decimal" or "numeric" or "money" or "smallmoney" => "decimal",
            "datetime" or "datetime2" or "smalldatetime" => "DateTime",
            "date" => "DateTime",
            "time" => "TimeSpan",
            "char" or "nchar" or "varchar" or "nvarchar" or "text" or "ntext" => "string",
            "uniqueidentifier" => "Guid",
            "binary" or "varbinary" or "image" => "byte[]",
            _ => "string",
        };

    public string GenerateDtoCode(DtoDefinition dto)
    {
        var ns = INamespace.New(dto.Namespace);
        var cls = IClass.New(dto.Name);
        foreach (var field in dto.Fields)
        {
            cls.AddProperty(field.Name, TypePath.New(field.Type));
        }

        ns.AddType(cls);

        var generator = new RoslynCodeGenerator();
        return generator.Generate(ns).GetValue();
    }
}
