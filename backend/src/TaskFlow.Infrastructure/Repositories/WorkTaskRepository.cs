using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Abstractions;
using TaskFlow.Application.Common.Models;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories
{
    public class WorkTaskRepository(TaskFlowDbContext dbContext)
    : IWorkTaskRepository
    {
        public async Task<bool> ProjectExistsAsync(
        Guid projectId,
        CancellationToken cancellationToken)
        {
            return await dbContext.Projects.AnyAsync(x => x.Id == projectId, cancellationToken);
        }

        public async Task<bool> MemberExistsAsync(
            Guid memberId,
            CancellationToken cancellationToken)
        {
            return await dbContext.TeamMembers
                .AnyAsync(x => x.Id == memberId, cancellationToken);
        }

        public async Task AddAsync(
            WorkTask workTask,
            CancellationToken cancellationToken)
        {
            await dbContext.WorkTasks.AddAsync(workTask, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<WorkTask>> GetAllAsync(
    CancellationToken cancellationToken)
        {
            return await dbContext.WorkTasks
                .AsNoTracking()
                .Include(x => x.Project)
                .Include(x => x.AssignedTo)
                .OrderBy(x => x.DueDate)
                .ToListAsync(cancellationToken);
        }

        public Task<WorkTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return dbContext.WorkTasks
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        public void Remove(WorkTask task, CancellationToken cancellationToken)
        {
            dbContext.WorkTasks.Remove(task);
        }

        public async Task<PagedResult<WorkTask>> GetPagedAsync(string? search, int? status, int page, int pageSize, CancellationToken cancellationToken)
        {
            IQueryable<WorkTask> query = dbContext.WorkTasks
       .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchText = search.Trim();

                query = query.Where(task =>
                    task.Title.Contains(searchText));
            }

            if (status.HasValue)
            {
                var selectedStatus = (WorkTaskStatus)status.Value;

                query = query.Where(task =>
                    task.Status == selectedStatus);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Include(task => task.Project)
                .Include(task => task.AssignedTo)
                .OrderBy(task => task.DueDate)
                .ThenBy(task => task.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<WorkTask>(
                items,
                totalCount,
                page,
                pageSize);
        }
    }
}