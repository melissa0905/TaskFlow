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
    public sealed class TeamMemberRepository(TaskFlowDbContext context) : ITeamMemberRepository
    {
        public async Task AddAsync(TeamMember member, CancellationToken cancellationToken)
        {
            await context.TeamMembers.AddAsync(member, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            return await context.TeamMembers.AsNoTracking().AnyAsync
            (m => m.Email == email, cancellationToken);
        }

        public async Task<IReadOnlyList<TeamMember>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await context.TeamMembers
            .AsNoTracking()
            .OrderBy(x => x.FullName)
            .ToListAsync(cancellationToken);
        }
    }
}