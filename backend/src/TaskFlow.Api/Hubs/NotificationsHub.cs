using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TaskFlow.Api.Hubs
{
    public sealed class NotificationsHub : Hub
    {
        public static string GroupName(Guid recipientId)
        {
            return $"notifications:{recipientId:D}";
        }

        public async Task Subscribe(Guid recipientId)
        {
            if (recipientId == Guid.Empty)
            {
                throw new HubException("Geçerli bir üye seçilmelidir.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(recipientId));
        }


    }
}