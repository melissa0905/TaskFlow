using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Abstractions;
using TaskFlow.Application.WorkTasks.Commands.CreateWorkTask;

namespace TaskFlow.Application.WorkTasks.Commands.CreateWorkTask
{
    public sealed class CreateWorkTaskCommandHandler(
    IWorkTaskRepository repository) : IRequestHandler<CreateWorkTaskCommand, Guid>
    {
        public async Task<Guid> Handle(CreateWorkTaskCommand request, CancellationToken cancellationToken)
        {
            if (!await repository.ProjectExistsAsync(
                    request.ProjectId, cancellationToken))
                throw new ArgumentException("Proje bulunamadı.");

            if (request.AssignedToId is Guid memberId &&
            !await repository.MemberExistsAsync(memberId, cancellationToken))
            throw new ArgumentException("Ekip üyesi bulunamadı.");

            var workTask = new Domain.Entities.WorkTask
            {
                ProjectId = request.ProjectId,
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                DueDate = request.DueDate,
                AssignedToId = request.AssignedToId
            };

            await repository.AddAsync(workTask, cancellationToken);

            return workTask.Id;
        }
    }
}
