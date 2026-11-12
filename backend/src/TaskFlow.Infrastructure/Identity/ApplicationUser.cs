using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Infrastructure.Identity
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid();
        }

        public string FullName { get; set; } = string.Empty;
        public Guid? TeamMemberId { get; set; }
        
        
    }
}