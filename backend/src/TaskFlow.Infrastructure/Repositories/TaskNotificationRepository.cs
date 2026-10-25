using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Abstractions;
using TaskFlow.Application.Common.Models;
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

        public async Task<PagedResult<TaskNotification>> GetPagedAsync(Guid recipientId, bool onlyUnread, int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = dbContext.Notifications
        .AsNoTracking()
        .Where(notification =>
            notification.ReciptientId == recipientId);

            if (onlyUnread)
            {
                query = query.Where(notification =>
                    !notification.IsRead);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(notification =>
                    notification.CreatedUtc)
                .ThenByDescending(notification =>
                    notification.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TaskNotification>(
                items,
                totalCount,
                page,
                pageSize);
        }

        public async Task<bool> MarkAsReadAsync(Guid notificationId, Guid recipientId, CancellationToken cancellationToken)
        {
            var affectedRows = await dbContext.Notifications
        .Where(notification =>
            notification.Id == notificationId &&
            notification.ReciptientId == recipientId)
        .ExecuteUpdateAsync(
            setters => setters.SetProperty(
                notification => notification.IsRead,
                true),
            cancellationToken);

            return affectedRows > 0;
        }
    }
}