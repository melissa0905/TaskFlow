using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Common.Models;

namespace TaskFlow.Application.Notifications.Queries.GetNotifications
{

    public sealed record GetNotificationsQuery(
        Guid RecipientId,
        bool OnlyUnread = false,
        int Page = 1,
        int PageSize = 10)
        : IRequest<PagedResult<NotificationListItem>>;
}