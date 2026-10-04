using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Abstractions;

namespace TaskFlow.Application.Projects.Queries.GetDashboardSummary
{
    public sealed class GetDashboardSummaryQueryHandler(IDashboardRepository dashboardRepository) : IRequestHandler<GetDashboardSummaryQuery, DashboardSummary>
    {
        public async Task<DashboardSummary> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
        {
            var todayUtc = DateTime.UtcNow;
            var summary = await dashboardRepository.GetSummaryAsync(todayUtc, cancellationToken);
            return summary;
        }
    }

}