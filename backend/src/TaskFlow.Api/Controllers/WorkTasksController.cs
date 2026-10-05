using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Contracts.WorkTasks;
using TaskFlow.Application.WorkTasks.Commands.CreateWorkTask;
using TaskFlow.Application.WorkTasks.Commands.DeleteWorkTask;
using TaskFlow.Application.WorkTasks.Commands.UpdateWorkTask;
using TaskFlow.Application.WorkTasks.Queries.GetWorkTaskById;
using TaskFlow.Application.WorkTasks.Queries.GetWorkTasks;
using TaskFlow.Domain.Enums;

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
        public async Task<IActionResult> GetAll(
    [FromQuery] string? search = null,
    [FromQuery] int? status = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
        {
            if (page < 1)
            {
                ModelState.AddModelError(
                    nameof(page),
                    "Sayfa numarası en az 1 olmalıdır.");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                ModelState.AddModelError(
                    nameof(pageSize),
                    "Sayfa boyutu 1 ile 100 arasında olmalıdır.");
            }

            if (status.HasValue &&
                !Enum.IsDefined(typeof(WorkTaskStatus), status.Value))
            {
                ModelState.AddModelError(
                    nameof(status),
                    "Geçersiz görev durumu.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await sender.Send(
                new GetWorkTasksQuery(search, status, page, pageSize),
                cancellationToken);

            return Ok(result);
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
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateWorkTaskRequest request,
        CancellationToken cancellationToken)
        {
            if (request.DueDate!.Value.Kind != DateTimeKind.Utc)
            {
                ModelState.AddModelError(
                    nameof(request.DueDate),
                    "Son tarih UTC olarak gönderilmelidir.");

                return ValidationProblem(ModelState);
            }

            var result = await sender.Send(
                new UpdateWorkTaskCommand(
                    id,
                    request.Title,
                    request.Description,
                    request.DueDate.Value,
                    request.AssignedToId),
                cancellationToken);

            if (result == UpdateWorkTaskResult.NotFound)
                return NotFound();

            if (result == UpdateWorkTaskResult.InvalidAssignee)
            {
                ModelState.AddModelError(
                    nameof(request.AssignedToId),
                    "Seçilen ekip üyesi bulunamadı.");

                return ValidationProblem(ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
        {
            var deleted = await sender.Send(
                new DeleteWorkTaskCommand(id),
                cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();

        }
    }
}