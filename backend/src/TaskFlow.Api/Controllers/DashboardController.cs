using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Projects.Queries.GetDashboardSummary;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController(ISender sender) : ControllerBase
    {
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary(CancellationToken cancellationToken)
        {
            var query = new GetDashboardSummaryQuery();
            var summary = await sender.Send(query, cancellationToken);
            return Ok(summary);
        }
        
    }
}