using MediatR;

namespace TaskFlow.Application.Projects.Queries.GetProjects;

public sealed record GetProjectsQuery()
    : IRequest<IReadOnlyList<ProjectListItem>>;