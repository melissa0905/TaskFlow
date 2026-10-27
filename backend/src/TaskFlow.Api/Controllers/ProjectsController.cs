using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Contracts.Projects;
using TaskFlow.Application.Projects.Commands.CreateProject;
using TaskFlow.Application.Projects.Queries.GetProjectById;
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
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetProjectByIdQuery(id),cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}