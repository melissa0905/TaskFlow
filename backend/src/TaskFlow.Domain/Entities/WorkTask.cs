using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities
{
    public class WorkTask
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public WorkTaskStatus Status { get; set; } = WorkTaskStatus.Todo;
        public DateTime DueDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        
        public Guid? AssignedToId { get; set; }
        public TeamMember? AssignedTo { get; set; }
    }
}