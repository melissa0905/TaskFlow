using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Abstractions;

namespace TaskFlow.Application.WorkTasks.Queries.GetWorkTasks
{
    public sealed class GetWorkTasksQueryHandler(
     IWorkTaskRepository repository
 ) : IRequestHandler<GetWorkTasksQuery, IReadOnlyList<WorkTaskListItem>>
    {
        public async Task<IReadOnlyList<WorkTaskListItem>> Handle(
            GetWorkTasksQuery request,
            CancellationToken cancellationToken)
        {
            var tasks = await repository.GetAllAsync(cancellationToken);

            return tasks
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
        }
    }
}