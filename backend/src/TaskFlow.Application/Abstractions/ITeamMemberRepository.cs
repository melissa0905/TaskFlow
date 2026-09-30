using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Abstractions
{
    public interface ITeamMemberRepository
    {
        Task AddAsync(TeamMember member,
         CancellationToken cancellationToken);

        Task<bool> EmailExistsAsync(string email,
         CancellationToken cancellationToken);

        Task<IReadOnlyList<TeamMember>> GetAllAsync(
        CancellationToken cancellationToken);
    }
}