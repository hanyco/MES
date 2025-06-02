using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Data.SqlServer;

public static class SqlEntity
{
    public static IEnumerable<ColumnInfo> GetColumns<TEntityType>()
    {
        return getColumns().OrderBy(x => x.Order);

        static IEnumerable<ColumnInfo> getColumns()
        {
            var entityType = typeof(TEntityType);
            var props = entityType.GetProperties();
            foreach (var prop in props)
            {
                var dbGen = ObjectHelper.GetAttribute<DatabaseGeneratedAttribute>(prop);
                if (dbGen?.DatabaseGeneratedOption is DatabaseGeneratedOption.None or DatabaseGeneratedOption.Computed)
                {
                    continue;
                }
                var colAttr = ObjectHelper.GetAttribute<ColumnAttribute>(prop);
                var fkAttr = ObjectHelper.GetAttribute<ForeignKeyAttribute>(prop);
                var keyAttr = ObjectHelper.GetAttribute<KeyAttribute>(prop);
                var name = colAttr?.Name is not null ? colAttr.Name : prop.Name;
                var isKey = keyAttr is not null || dbGen?.DatabaseGeneratedOption is DatabaseGeneratedOption.Identity;
                var isFk = fkAttr is not null;
                var fkName = fkAttr?.Name;
                var type = colAttr?.TypeName is not null ? SqlTypeHelper.SqlTypeToNetType(colAttr.TypeName) : prop.PropertyType;
                var order = colAttr?.Order is not null ? colAttr.Order : int.MaxValue;
                yield return new ColumnInfo(name, type, isKey, isFk, fkName, order);
            }
        }
    }

    public static TableInfo GetTableInfo<TEntity>()
        => new(GetTableName<TEntity>(), GetColumns<TEntity>());

    public static string GetTableName<TEntity>() =>
        GetTableName(typeof(TEntity));

    public static string GetTableName(Type tableType)
    {
        Checker.MustBeArgumentNotNull(tableType);
        var tableAttr = ObjectHelper.GetAttribute<TableAttribute>(tableType);
        return tableAttr is not null
            ? tableAttr.Schema.IsNullOrEmpty() ? tableAttr.Name : $"{tableAttr.Schema}.{tableAttr.Name}"
            : tableType.Name;
    }
}

public readonly record struct ColumnInfo(string Name, Type Type, bool IsKey, bool IsForeignKey, string? ForeignKeyName, int Order);
public readonly record struct TableInfo(string Name, IEnumerable<ColumnInfo> Columns);