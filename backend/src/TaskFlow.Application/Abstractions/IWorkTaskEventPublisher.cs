using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskFlow.Application.Abstractions
{
    public interface IWorkTaskEventPublisher
    {
        Task PublishAssignedAsync(
            Guid taskId,
            Guid assignedId,
            string title,
            CancellationToken cancellationToken);   
    }
}