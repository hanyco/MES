using System.Diagnostics.CodeAnalysis;

using DataLib;

using Library.Resulting;

namespace CodeGenerator.Application.Services;

public interface IPropertyService : ICrudService<Property>
{
    Task<IResult> DeleteByParentId(long dtoId, CancellationToken ct = default);

    [return: NotNull]
    Property GetByTableField([DisallowNull] Field tableField);
}
