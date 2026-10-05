using MediatR;
using TaskFlow.Application.Common.Models;

namespace TaskFlow.Application.WorkTasks.Queries.GetWorkTasks;

public sealed record GetWorkTasksQuery(
    string? Search = null,
    int? Status = null,
    int Page = 1,
    int PageSize = 10)
    : IRequest<PagedResult<WorkTaskListItem>>;