using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Abstractions;
using TaskFlow.Application.Common.Models;

namespace TaskFlow.Application.Notifications.Queries.GetNotifications
{
    public sealed class GetNotificationsQueryHandler(ITaskNotificationRepository repository) : IRequestHandler<
        GetNotificationsQuery,
        PagedResult<NotificationListItem>>
    {
        public async Task<PagedResult<NotificationListItem>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetPagedAsync(
            request.RecipientId,
            request.OnlyUnread,
            request.Page,
            request.PageSize,
            cancellationToken);

        var items = result.Items
            .Select(notification => new NotificationListItem(
                notification.Id,
                notification.TaskId,
                notification.Message,
                notification.IsRead,
                notification.CreatedUtc))
            .ToList();

        return new PagedResult<NotificationListItem>(
            items,
            result.TotalCount,
            result.Page,
            result.PageSize);
        }
    }
}