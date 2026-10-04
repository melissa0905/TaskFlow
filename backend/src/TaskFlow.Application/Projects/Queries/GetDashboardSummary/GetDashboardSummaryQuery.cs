using MediatR;

namespace TaskFlow.Application.Projects.Queries.GetDashboardSummary;

public sealed record class GetDashboardSummaryQuery() : IRequest<DashboardSummary>;
