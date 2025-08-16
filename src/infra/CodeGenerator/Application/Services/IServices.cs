using CodeGenerator.Application.Domain;
using Library.Resulting;

namespace CodeGenerator.Application.Services;

public interface IDtoService : ICodeGenerator<Dto>, ICrudService<Dto>;

public interface IPropertyService : ICrudService<Property>
{
    Task<IResult> DeleteByDtoId(long dtoId, CancellationToken ct = default);
}

public interface IModuleService : IViewService<Module>;
