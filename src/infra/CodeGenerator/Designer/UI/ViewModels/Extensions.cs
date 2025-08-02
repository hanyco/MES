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
        : Copy<DtoViewModel, Dto>(@this)
            .With(x => x.ModuleId = @this.Module?.Id)
            .With(x => x.DbObjectId = @this.ObjectId.ToString())
            .With(x => x.Properties = [.. @this.Properties
                        .Select(Copy<Property, Property>)!
                        .ForEach<IEnumerable<Property>, Property>(x =>
                        {
                            x.PropertyType = x.TypeFullName.IsNullOrEmpty()
                                ? TypePath.GetTypeCode(nameof(Object))
                                : SqlType.IsSqlType(x.TypeFullName)
                                    ? SqlType.ToSqlType(x.TypeFullName).GetTypeCode()
                                    : TypePath.GetTypeCode(x.TypeFullName);
                        })]);

    [return: NotNullIfNotNull(nameof(@this))]
    [return: NotNullIfNotNull(nameof(dest))]
    private static TDest? Copy<TSource, TDest>(TSource? @this, TDest? dest, bool throwExtension = false)
        where TSource : class
        where TDest : class
    {
        if (@this is null || dest is null)
        {
            return dest;
        }
        var destProperties = typeof(TDest).GetProperties();
        foreach (var prop in typeof(TSource).GetProperties())
        {
            try
            {
                destProperties
                    .FirstOrDefault(x => x.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase))?
                    .SetValue(dest, prop.GetValue(@this));
            }
            catch (Exception)
            {
                if (throwExtension)
                {
                    throw;
                }
            }
        }
        return dest;
    }

    [return: NotNullIfNotNull(nameof(@this))]
    private static TDest? Copy<TSource, TDest>(TSource? @this)
        where TSource : class
        where TDest : class, new() => Copy<TSource, TDest>(@this, new());
}