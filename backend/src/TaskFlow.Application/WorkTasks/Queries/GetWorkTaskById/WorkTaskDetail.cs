namespace TaskFlow.Application.WorkTasks.Queries.GetWorkTaskById;

public sealed record WorkTaskDetail(
    Guid Id,
    string Title,
    string Description,
    int Status,
    DateTime DueDate,
    Guid ProjectId,
    Guid? AssignedToId);