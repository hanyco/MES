using DataLib;

namespace CodeGenerator.Application.Domain;

internal static class Extensions
{
    extension(Property)
    {
        public static Property GetByTableField(Field tableField) => new()
        {
            DbObjectId = tableField.ObjectId.ToString(),
            Comment = tableField.Comment,
            IsNullable = tableField.AllowNull,
            Name = tableField.Name,
            TypeFullName = tableField.Type
        };
    }
}