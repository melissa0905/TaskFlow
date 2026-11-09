using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskFlow.Application.Auth
{
    public sealed record RegistrationResult
    (
        bool Succeeded,
        Guid? UserId,
        IReadOnlyList<string> Errors
    );
}