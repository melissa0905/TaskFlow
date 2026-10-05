using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Abstractions;
using TaskFlow.Application.Common.Models;

namespace TaskFlow.Application.WorkTasks.Queries.GetWorkTasks
{
    public sealed class GetWorkTasksQueryHandler(
    IWorkTaskRepository repository)
    : IRequestHandler<
        GetWorkTasksQuery,
        PagedResult<WorkTaskListItem>>
    {
        public async Task<PagedResult<WorkTaskListItem>> Handle(
            GetWorkTasksQuery request,
            CancellationToken cancellationToken)
        {
            var result = await repository.GetPagedAsync(
                request.Search,
                request.Status,
                request.Page,
                request.PageSize,
                cancellationToken);

            var items = result.Items
                .Select(task => new WorkTaskListItem(
                    task.Id,
                    task.Title,
                    task.Description,
                    (int)task.Status,
                    task.DueDate,
                    task.ProjectId,
                    task.Project.Name,
                    task.AssignedToId,
                    task.AssignedTo?.FullName))
                .ToList();

            return new PagedResult<WorkTaskListItem>(
                items,
                result.TotalCount,
                result.Page,
                result.PageSize);
        }
    }
}