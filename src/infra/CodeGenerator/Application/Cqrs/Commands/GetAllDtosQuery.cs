using CodeGenerator.Application.Domain;

using MediatR;

namespace CodeGenerator.Application.Cqrs.Commands;
/// <summary>
/// Query to retrieve all DTO definitions.
/// </summary>
public record GetAllDtosQuery() : IRequest<IEnumerable<DtoDefinition>>;