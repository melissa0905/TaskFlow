using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.Application.Projects.Queries.GetDashboardSummary;

namespace TaskFlow.Application.Abstractions
{
    public interface IDashboardRepository
    {
      Task<DashboardSummary> GetSummaryAsync(
      DateTime todayUtc,
      CancellationToken cancellationToken);
    }
}