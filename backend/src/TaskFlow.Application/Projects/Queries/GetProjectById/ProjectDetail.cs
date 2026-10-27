using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskFlow.Application.Projects.Queries.GetProjectById
{
    public sealed record ProjectDetail(
    Guid Id,
    string Name,
    string? Description);
}