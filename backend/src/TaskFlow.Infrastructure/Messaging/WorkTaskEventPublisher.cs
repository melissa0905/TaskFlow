using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using TaskFlow.Application.Abstractions;
using TaskFlow.Contracts.Events;

namespace TaskFlow.Infrastructure.Messaging
{
    public sealed class WorkTaskEventPublisher(IPublishEndpoint publishEndpoint) : IWorkTaskEventPublisher
    {
        public Task PublishAssignedAsync(Guid taskId, Guid assignedId, string title, CancellationToken cancellationToken)
        {
            var message =new WorkTaskAssigned(
                EventId: Guid.NewGuid(),
                TaskId: taskId,
                AssignedId: assignedId,
                Title: title,
                OccuredAtUtc: DateTime.UtcNow);


            return publishEndpoint.Publish(message, cancellationToken);
        }
    }
}