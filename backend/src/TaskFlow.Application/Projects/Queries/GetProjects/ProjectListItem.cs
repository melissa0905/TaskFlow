namespace TaskFlow.Application.Projects.Queries.GetProjects;

public sealed record ProjectListItem(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAtUtc);
