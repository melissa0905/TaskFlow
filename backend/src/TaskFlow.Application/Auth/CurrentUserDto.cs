using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskFlow.Application.Auth
{
    public sealed record CurrentUserDto
    (
        Guid Id,
        string FullName,
        string Email,
        IReadOnlyList<string> Roles
    );

}