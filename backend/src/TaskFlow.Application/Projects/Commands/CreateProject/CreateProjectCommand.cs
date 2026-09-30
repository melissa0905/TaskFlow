using MediatR;

namespace TaskFlow.Application.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(
    string Name,
    string? Description
) : IRequest<Guid>;
