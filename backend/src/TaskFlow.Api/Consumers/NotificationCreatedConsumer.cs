using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using TaskFlow.Api.Hubs;
using TaskFlow.Contracts.Events;

namespace TaskFlow.Api.Consumers
{
    public class NotificationCreatedConsumer(IHubContext<NotificationsHub> hubContext) : IConsumer<NotificationCreated>
    {
        public Task Consume(ConsumeContext<NotificationCreated> context)
        {
            var groupName = NotificationsHub.GroupName(
           context.Message.RecipientId);
          
          return hubContext.Clients.Group(groupName).SendAsync("NotificationsChanged",cancellationToken:context.CancellationToken);
        }
    }
}