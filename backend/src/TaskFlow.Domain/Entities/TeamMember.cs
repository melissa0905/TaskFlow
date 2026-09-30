using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskFlow.Domain.Entities
{
    public class TeamMember
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public ICollection<WorkTask> AssignedTasks { get; set; } = new List<WorkTask>();
    }
}