using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Abstractions;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories
{
    public class TaskNotificationRepository(TaskFlowDbContext dbContext) : ITaskNotificationRepository
    {
        public async Task<bool> AddIfNotExistsAsync(TaskNotification notification, CancellationToken cancellationToken)
        {
             var affectedRows =
            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"""
                INSERT INTO taskflow.notifications
                (
                    "Id",
                    "EventId",
                    "TaskId",
                    "ReciptientId",
                    "Message",
                    "IsRead",
                    "CreatedUtc"
                )
                VALUES
                (
                    {notification.Id},
                    {notification.EventId},
                    {notification.TaskId},
                    {notification.ReciptientId},
                    {notification.Message},
                    {notification.IsRead},
                    {notification.CreatedUtc}
                )
                ON CONFLICT ("EventId") DO NOTHING;
                """,
                cancellationToken);

        return affectedRows == 1;
        }
    }
}