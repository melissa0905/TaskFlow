using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.Application.Common.Models;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Abstractions
{
    public interface ITaskNotificationRepository
    {
        Task<bool> AddIfNotExistsAsync(
      TaskNotification notification,
      CancellationToken cancellationToken);

        Task<PagedResult<TaskNotification>> GetPagedAsync(
       Guid recipientId,
       bool onlyUnread,
       int page,
       int pageSize,
       CancellationToken cancellationToken);

        Task<bool> MarkAsReadAsync(
            Guid notificationId,
            Guid recipientId,
            CancellationToken cancellationToken);
    }
}