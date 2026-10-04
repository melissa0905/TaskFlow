using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Abstractions;
using TaskFlow.Application.Projects.Queries.GetDashboardSummary;
using TaskFlow.Domain.Enums;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories
{
    public sealed class DashboardRepository(TaskFlowDbContext dbContext) : IDashboardRepository
    {
        public async Task<DashboardSummary> GetSummaryAsync(DateTime todayUtc, CancellationToken cancellationToken)
        {
            var tasks = dbContext.WorkTasks.AsNoTracking();
            var total = await tasks.CountAsync(cancellationToken);
            var inProgress = await tasks.CountAsync(t => t.Status == WorkTaskStatus.InProgress, cancellationToken);
            var completed = await tasks.CountAsync(t => t.Status == WorkTaskStatus.Done, cancellationToken);
            var overdue = await tasks.CountAsync(t => t.DueDate < todayUtc && t.Status != WorkTaskStatus.Done, cancellationToken);
            
            return new DashboardSummary(
                TotalTasks: total,
                InProgressTasks: inProgress,
                CompletedTasks: completed,
                OverdueTasks: overdue);
        }   
    }
}