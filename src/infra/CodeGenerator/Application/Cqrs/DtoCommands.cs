using CodeGenerator.Application.Domain;

using MediatR;

namespace CodeGenerator.Application.Cqrs;
/// <summary>
/// Command to delete a DTO definition by its identifier.
/// </summary>
public record DeleteDtoCommand(Guid Id) : IRequest;

/// <summary>
/// Command to create a new DTO definition.
/// </summary>
public record CreateDtoCommand(DtoDefinition Dto) : IRequest<DtoDefinition>;

/// <summary>
/// Command to update an existing DTO definition.
/// </summary>
public record UpdateDtoCommand(DtoDefinition Dto) : IRequest<DtoDefinition>;