using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskFlow.Api.Contracts.Auth
{
    public sealed record LoginRequest(
    [property: Required, EmailAddress, StringLength(256)]
    string Email,

    [property: Required, StringLength(128)]
    string Password);
}