using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.Application.Common.Models;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Abstractions
{
    public interface IWorkTaskRepository
    {
        Task<bool> ProjectExistsAsync(Guid projectId, CancellationToken cancellationToken);
        Task<bool> MemberExistsAsync(Guid memberId, CancellationToken cancellationToken);
        Task AddAsync(WorkTask workTask, CancellationToken cancellationToken);
        Task<IReadOnlyList<WorkTask>> GetAllAsync(
    CancellationToken cancellationToken);

        Task<WorkTask?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

        Task SaveChangesAsync(
            CancellationToken cancellationToken);
        void Remove(WorkTask task, CancellationToken cancellationToken);

        Task<PagedResult<WorkTask>> GetPagedAsync(
    string? search,
    int? status,
    int page,
    int pageSize,
    CancellationToken cancellationToken);
    }
}