using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace TaskFlow.Application.Notifications.Commands.MarkNotificationAsRead
{
   public sealed record MarkNotificationAsReadCommand(
    Guid NotificationId,
    Guid RecipientId)
    : IRequest<bool>;
}