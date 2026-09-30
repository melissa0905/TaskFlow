using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Contracts.Projects;
using TaskFlow.Application.Projects.Commands.CreateProject;
using TaskFlow.Application.Projects.Queries.GetProjects;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController(ISender sender) : ControllerBase
    {
       [HttpPost]
       public async Task<IActionResult> Create(
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken)
        {
            var id = await sender.Send(
                new CreateProjectCommand(request.Name, request.Description),
                cancellationToken);

            return Created($"/api/projects/{id}", new { id });
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var projects = await sender.Send(new GetProjectsQuery(), cancellationToken);
            return Ok(projects);
        }

    }
}