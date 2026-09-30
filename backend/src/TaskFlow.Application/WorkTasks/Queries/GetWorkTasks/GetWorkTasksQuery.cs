using MediatR;

namespace TaskFlow.Application.WorkTasks.Queries.GetWorkTasks;

public sealed record GetWorkTasksQuery()
    : IRequest<IReadOnlyList<WorkTaskListItem>>;
