using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Library.Data.SqlServer.ObjectModel;
using Library.Extensions;

namespace Library.Data.SqlServer;

internal static class InternalServices
{
    internal static TableSchema Refactor(Type tableEntity, bool includeAutoIcreamentals) => RefactorCore(tableEntity, includeAutoIcreamentals);

    internal static TableSchema Refactor(object obj, bool includeAutoIcreamentals) => RefactorCore(obj.GetType(),
        includeAutoIcreamentals,
        p =>
        {
            var value = p.GetValue(obj);
            if (p.PropertyType == typeof(string) || p.PropertyType == typeof(Guid))
            {
                value = $"N'{value}'";
            }
            else if (p.PropertyType == typeof(bool))
            {
                value = $"{((bool)value ? 1 : 0)}";
            }

            return value;
        });

    internal static TableSchema Refactor(string tableName, IEnumerable<Tuple<string, object, Type>> props)
    {
        var pairs = new List<(string Value1, object Value2)>(props.Count());
        foreach (var prop in props)
        {
            (string Value1, object Value2) item = new();
            if (prop.Item3 == typeof(string) || prop.Item3 == typeof(Guid) || prop.Item3 == typeof(Guid?))
            {
                item.Value2 = $"N'{prop.Item2}'";
            }
            else if (prop.Item3 == typeof(bool) || prop.Item3 == typeof(bool?))
            {
                item.Value2 = $"{((bool)prop.Item2 ? 1 : 0)}";
            }
            else
            {
                item.Value2 = $"{prop.Item2}";
            }

            item.Value1 = $"[{prop.Item1}]";
            pairs.Add(item);
        }

        return new TableSchema
        {
            Columns = pairs,
            TableName = tableName
        };
    }

    internal static TableSchema Refactor(object obj) => Refactor(obj.GetType().Name,
        obj.GetType()
            .GetProperties()
            .Select(prop => new Tuple<string, object, Type>(prop.Name, prop.GetValue(obj), prop.PropertyType)));

    internal static TableSchema RefactorCore(Type tableEntity, bool includeAutoIcreamentals, Func<PropertyInfo, object>? getValue = null)
    {
        if (getValue == null)
        {
            getValue = p => null;
        }

        var tableAttr = tableEntity.GetCustomAttributes(typeof(TableAttribute), true).Cast<TableAttribute>().FirstOrDefault();
        var tablename = tableAttr != null ? tableAttr.Name : tableEntity.Name;
        var columns = new List<(string Value1, object Value2)>();
        foreach (var property in tableEntity.GetProperties()
            //.Where(property =>
            //    property.CustomAttributes.FirstOrDefault(
            //        attr => attr.AttributeType == typeof(SqlIgnoreAttribute)) == null)
            )
        {
            if (!includeAutoIcreamentals)
            {
                if (property.CustomAttributes.FirstOrDefault(attr => attr.AttributeType == typeof(SqlAutoIcreamentalAttribute)) != null)
                {
                    continue;
                }
            }
            var attrdata = property.CustomAttributes.FirstOrDefault(attr => attr.AttributeType == typeof(ColumnAttribute));
            if (attrdata == null)
            {
                columns.Add(new(string.Concat("[", property.Name, "]"), getValue(property)));
            }
            else
            {
                var fieldAttr = attrdata.AttributeType;
                columns.Add(new(fieldAttr != null ? fieldAttr.Name.IfNullOrEmpty(property.Name) : property.Name,
                    getValue(property)));
            }
        }

        return new TableSchema
        {
            TableName = tablename,
            Columns = columns.AsEnumerable()
        };
    }

    internal class TableSchema
    {
        public IEnumerable<string> ColumnNames => this.Columns.Select(c => c.Value1);

        public IEnumerable<(string Value1, object Value2)> Columns { get; set; }
        public string TableName { get; set; }
    }
}
