using MediatR;

namespace TaskFlow.Application.WorkTasks.Commands.DeleteWorkTask;

public sealed record DeleteWorkTaskCommand(Guid Id) : IRequest<bool>;