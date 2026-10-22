using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Abstractions
{
    public interface ITaskNotificationRepository
    {
          Task<bool> AddIfNotExistsAsync(
        TaskNotification notification,
        CancellationToken cancellationToken);
    }
}