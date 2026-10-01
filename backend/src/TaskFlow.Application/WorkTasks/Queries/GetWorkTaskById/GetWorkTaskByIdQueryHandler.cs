using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Abstractions;

namespace TaskFlow.Application.WorkTasks.Queries.GetWorkTaskById
{
    public sealed class GetWorkTaskByIdQueryHandler(IWorkTaskRepository repository) : IRequestHandler<GetWorkTaskByIdQuery, WorkTaskDetail?>
    {
        public async Task<WorkTaskDetail?> Handle(
            GetWorkTaskByIdQuery request,
            CancellationToken cancellationToken)
        {
            var task = await repository.GetByIdAsync(request.Id, cancellationToken);

            if (task is null)
                return null;

            return new WorkTaskDetail(
                task.Id,
                task.Title,
                task.Description,
                (int)task.Status,
                task.DueDate,
                task.ProjectId,
                task.AssignedToId);
        }
    }
}