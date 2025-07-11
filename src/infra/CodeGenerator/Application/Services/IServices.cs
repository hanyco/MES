using CodeGenerator.Application.Domain;

namespace CodeGenerator.Application.Services;

public interface IDtoService : ICodeGenerator<Dto>, ICrudService<Dto>;

public interface IModuleService : IViewService<Module>;