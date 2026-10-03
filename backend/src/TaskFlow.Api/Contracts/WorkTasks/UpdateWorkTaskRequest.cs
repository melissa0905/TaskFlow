using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.Contracts.WorkTasks;

public sealed record UpdateWorkTaskRequest(
    [Required, StringLength(200)] string Title,
    [Required, StringLength(2000)] string Description,
    [Required] DateTime? DueDate,
    Guid? AssignedToId);
