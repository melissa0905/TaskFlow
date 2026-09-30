using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Abstractions;

namespace TaskFlow.Application.WorkTasks.Commands.CreateWorkTask
{
    public class ChangeWorkTaskStatusCommandHandler(
    IWorkTaskRepository repository) : IRequestHandler<ChangeWorkTaskStatusCommand, bool>
    {
        public async Task<bool> Handle(
       ChangeWorkTaskStatusCommand request,
       CancellationToken cancellationToken)
        {
            var task = await repository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (task is null)
                return false;

            task.Status = request.Status;

            await repository.SaveChangesAsync(cancellationToken);

            return true;
        }

    }
}