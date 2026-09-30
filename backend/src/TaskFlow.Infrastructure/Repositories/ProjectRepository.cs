using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Abstractions;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories
{
    public sealed class ProjectRepository(TaskFlowDbContext dbContext) : IProjectRepository
    {
        public async Task AddAsync(Project project, CancellationToken cancellationToken)
        {
            await dbContext.Projects.AddAsync(project, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await dbContext.Projects
            .AsNoTracking().OrderByDescending(p => p.CreatedAtUtc).ToListAsync(cancellationToken);  
        }
    }
}