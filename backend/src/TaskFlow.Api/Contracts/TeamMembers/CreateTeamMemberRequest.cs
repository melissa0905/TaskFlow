namespace TaskFlow.Api.Contracts.TeamMembers;

public sealed record CreateTeamMemberRequest(
    string FullName,
    string Email);
