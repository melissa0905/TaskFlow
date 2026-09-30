using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Abstractions
{
    public interface IProjectRepository
    {
        Task AddAsync(Project project, CancellationToken cancellationToken);
        Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken);
    }
}