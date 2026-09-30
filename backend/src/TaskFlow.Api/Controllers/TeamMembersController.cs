using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.TeamMembers.Commands.CreateTeamMember;
using TaskFlow.Application.TeamMembers.Queries.GetTeamMembers;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/team-members")]
    public class TeamMembersController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateTeamMember(
             [FromBody] Contracts.TeamMembers.CreateTeamMemberRequest request,
             CancellationToken cancellationToken)
        {
            var id = await sender.Send(new CreateTeamMemberCommand(request.FullName, request.Email), cancellationToken);
            return Created($"/api/team-members/{id}", new { id });
        }
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<TeamMemberListItem>>> GetAll(
        CancellationToken cancellationToken)
        {
            var members = await sender.Send(
                new GetTeamMembersQuery(),
                cancellationToken);

            return Ok(members);
        }
    }
}