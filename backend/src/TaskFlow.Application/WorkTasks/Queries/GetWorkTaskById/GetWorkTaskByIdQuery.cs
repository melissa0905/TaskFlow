using MediatR;

namespace TaskFlow.Application.WorkTasks.Queries.GetWorkTaskById;

public sealed record GetWorkTaskByIdQuery(Guid Id)
    : IRequest<WorkTaskDetail?>;