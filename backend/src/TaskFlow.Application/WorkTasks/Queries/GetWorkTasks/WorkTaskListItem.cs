namespace TaskFlow.Application.WorkTasks.Queries.GetWorkTasks;

public sealed record WorkTaskListItem(
    Guid Id,
    string Title,
    string Description,
    int Status,
    DateTime DueDate,
    Guid ProjectId,
    string ProjectName,
    Guid? AssignedToId,
    string? AssigneeName);