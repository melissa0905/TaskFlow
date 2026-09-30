using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Abstractions;

namespace TaskFlow.Application.TeamMembers.Queries.GetTeamMembers
{
    public class GetTeamMembersQueryHandler(
    ITeamMemberRepository repository) : IRequestHandler<GetTeamMembersQuery, IReadOnlyList<TeamMemberListItem>>
    {
        public async Task<IReadOnlyList<TeamMemberListItem>> Handle(
        GetTeamMembersQuery request,
        CancellationToken cancellationToken)
    {
        var members = await repository.GetAllAsync(cancellationToken);

        return members
            .Select(member => new TeamMemberListItem(
                member.Id,
                member.FullName,
                member.Email))
            .ToList();
    }
    }
}