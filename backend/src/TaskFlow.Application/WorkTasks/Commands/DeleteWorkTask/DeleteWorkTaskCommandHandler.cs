using System;
using MediatR;
using TaskFlow.Application.Abstractions;

namespace TaskFlow.Application.WorkTasks.Commands.DeleteWorkTask;

public sealed class DeleteWorkTaskCommandHandler(IWorkTaskRepository workTaskRepository) : IRequestHandler<DeleteWorkTaskCommand, bool>
{
    public Task<bool> Handle(DeleteWorkTaskCommand request, CancellationToken cancellationToken)
    {
        var task = workTaskRepository.GetByIdAsync(request.Id, cancellationToken).Result;

        if (task is null)
            return Task.FromResult(false);

        workTaskRepository.Remove(task, cancellationToken);
        workTaskRepository.SaveChangesAsync(cancellationToken).Wait();

        return Task.FromResult(true);
    }
}
