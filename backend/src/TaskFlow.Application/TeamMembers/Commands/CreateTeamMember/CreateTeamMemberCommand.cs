using MediatR;

namespace TaskFlow.Application.TeamMembers.Commands.CreateTeamMember;

public sealed record CreateTeamMemberCommand(
    string FullName,
    string Email
) : IRequest<Guid>;
