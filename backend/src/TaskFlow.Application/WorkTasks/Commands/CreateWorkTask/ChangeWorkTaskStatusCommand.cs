using MediatR;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.WorkTasks.Commands.CreateWorkTask;


public sealed record ChangeWorkTaskStatusCommand(
    Guid Id,
    WorkTaskStatus Status
) : IRequest<bool>;