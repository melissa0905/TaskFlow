using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Api.Contracts.WorkTasks
{
   public sealed record ChangeWorkTaskStatusRequest(
    [Required, EnumDataType(typeof(WorkTaskStatus))]
    WorkTaskStatus? Status);
}
