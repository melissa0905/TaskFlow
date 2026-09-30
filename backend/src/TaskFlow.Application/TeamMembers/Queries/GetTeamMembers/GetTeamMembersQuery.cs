using MediatR;

namespace TaskFlow.Application.TeamMembers.Queries.GetTeamMembers;

public sealed record GetTeamMembersQuery()
    : IRequest<IReadOnlyList<TeamMemberListItem>>;