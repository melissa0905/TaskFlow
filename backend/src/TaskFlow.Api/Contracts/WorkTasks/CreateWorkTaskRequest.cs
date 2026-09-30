using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskFlow.Api.Contracts.WorkTasks
{
    public sealed record CreateWorkTaskRequest(
    Guid ProjectId,
    string Title,
    string Description,
    DateTime DueDate,
    Guid? AssignedToId);
}