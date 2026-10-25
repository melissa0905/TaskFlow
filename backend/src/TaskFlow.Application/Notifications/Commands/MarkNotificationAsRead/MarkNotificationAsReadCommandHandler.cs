using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Abstractions;

namespace TaskFlow.Application.Notifications.Commands.MarkNotificationAsRead
{
    public sealed class MarkNotificationAsReadCommandHandler(
     ITaskNotificationRepository repository)
     : IRequestHandler<MarkNotificationAsReadCommand, bool>
    {
        public Task<bool> Handle(
            MarkNotificationAsReadCommand request,
            CancellationToken cancellationToken)
        {
            return repository.MarkAsReadAsync(
                request.NotificationId,
                request.RecipientId,
                cancellationToken);
        }
    }
}