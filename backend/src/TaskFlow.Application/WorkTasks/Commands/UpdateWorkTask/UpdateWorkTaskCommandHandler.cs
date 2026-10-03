using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Abstractions;

namespace TaskFlow.Application.WorkTasks.Commands.UpdateWorkTask
{
    public sealed class UpdateWorkTaskCommandHandler(IWorkTaskRepository workTaskRepository) : IRequestHandler<UpdateWorkTaskCommand, UpdateWorkTaskResult>
    {
        public async Task<UpdateWorkTaskResult> Handle(UpdateWorkTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await workTaskRepository.GetByIdAsync(
           request.Id, cancellationToken);

            if (task is null)
                return UpdateWorkTaskResult.NotFound;

            if (request.AssignedToId is Guid memberId &&
                !await workTaskRepository.MemberExistsAsync(memberId, cancellationToken))
            {
                return UpdateWorkTaskResult.InvalidAssignee;
            }

            task.Title = request.Title.Trim();
            task.Description = request.Description.Trim();
            task.DueDate = request.DueDate;
            task.AssignedToId = request.AssignedToId;

            await workTaskRepository.SaveChangesAsync(cancellationToken);

            return UpdateWorkTaskResult.Updated;
        }
    }
}