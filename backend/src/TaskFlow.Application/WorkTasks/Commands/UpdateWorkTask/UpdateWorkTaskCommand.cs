using MediatR;

namespace TaskFlow.Application.WorkTasks.Commands.UpdateWorkTask;

public sealed record UpdateWorkTaskCommand(
    Guid Id,
    string Title,
    string Description,
    DateTime DueDate,
    Guid? AssignedToId
) : IRequest<UpdateWorkTaskResult>;

public enum UpdateWorkTaskResult
{
    Updated,
    NotFound,
    InvalidAssignee
}