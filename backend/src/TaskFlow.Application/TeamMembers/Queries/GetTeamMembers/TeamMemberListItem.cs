namespace TaskFlow.Application.TeamMembers.Queries.GetTeamMembers;

public sealed record TeamMemberListItem(
    Guid Id,
    string FullName,
    string Email);