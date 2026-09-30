using MediatR;

namespace TaskFlow.Application.WorkTasks.Commands.CreateWorkTask;

public sealed record CreateWorkTaskCommand(
    Guid ProjectId,
    string Title,
    string Description,
    DateTime DueDate,
    Guid? AssignedToId
) : IRequest<Guid>;