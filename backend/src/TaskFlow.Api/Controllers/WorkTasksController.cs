using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Contracts.WorkTasks;
using TaskFlow.Application.WorkTasks.Commands.CreateWorkTask;
using TaskFlow.Application.WorkTasks.Queries.GetWorkTaskById;
using TaskFlow.Application.WorkTasks.Queries.GetWorkTasks;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/work-tasks")]
    public class WorkTasksController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(
       [FromBody] CreateWorkTaskRequest request,
       CancellationToken cancellationToken)
        {
            var id = await sender.Send(
                new CreateWorkTaskCommand(
                    request.ProjectId,
                    request.Title,
                    request.Description,
                    request.DueDate,
                    request.AssignedToId),
                cancellationToken);

            return Created($"/api/work-tasks/{id}", new { id });
        }


        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<WorkTaskListItem>>> GetAll(
    CancellationToken cancellationToken)
        {
            var tasks = await sender.Send(
                new GetWorkTasksQuery(),
                cancellationToken);

            return Ok(tasks);
        }
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> ChangeStatus(
         Guid id,
        [FromBody] ChangeWorkTaskStatusRequest request,
        CancellationToken cancellationToken)
        {
            var updated = await sender.Send(
                new ChangeWorkTaskStatusCommand(
                    id,
                    request.Status!.Value),
                cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<WorkTaskDetail>> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var task = await sender.Send(
                new GetWorkTaskByIdQuery(id),
                cancellationToken);

            if (task is null)
                return NotFound();

            return Ok(task);
        }
    }
}