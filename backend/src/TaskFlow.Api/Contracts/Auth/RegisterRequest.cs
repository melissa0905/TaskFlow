using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskFlow.Api.Contracts.Auth
{
   public sealed record RegisterRequest(
    [property: Required, StringLength(150)]
    string FullName,

    [property: Required, EmailAddress, StringLength(256)]
    string Email,

    [property: Required, StringLength(128, MinimumLength = 12)]
    string Password);
}