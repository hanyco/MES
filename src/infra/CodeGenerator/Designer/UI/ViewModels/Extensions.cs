using System.Diagnostics.CodeAnalysis;

using DataLib.Extensions;
using DataLib.SqlServer;

using Library.Coding;

namespace CodeGenerator.Designer.UI.ViewModels;

public static class Extensions
{
    [return: NotNullIfNotNull(nameof(@this))]
    public static Dto? ToEntity(this DtoViewModel? @this)
        => @this is null
            ? null
            : Reflection.Copy<DtoViewModel, Dto>(@this)
                .With(x => x.ModuleId = @this.Module?.Id)
                .With(x => x.DbObjectId = @this.ObjectId.ToString())
                .With(x => x.Properties = [.. @this.Properties
                            .Select(Reflection.Copy<Property, Property>)!
                            .ForEach<IEnumerable<Property>, Property>(x =>
                            {
                                x.PropertyType = x.TypeFullName.IsNullOrEmpty()
                                    ? TypePath.GetTypeCode(nameof(Object))
                                    : SqlType.IsSqlType(x.TypeFullName)
                                        ? SqlType.ToSqlType(x.TypeFullName).GetTypeCode()
                                        : TypePath.GetTypeCode(x.TypeFullName);
                            })]);
}