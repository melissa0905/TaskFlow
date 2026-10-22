using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskFlow.Domain.Entities
{
    public class TaskNotification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid EventId { get; set; }
        public Guid TaskId { get; set; }
        public Guid ReciptientId { get; set; }
        public string Message { get; set; }=string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }
}