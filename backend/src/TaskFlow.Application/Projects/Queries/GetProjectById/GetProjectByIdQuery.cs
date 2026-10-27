using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace TaskFlow.Application.Projects.Queries.GetProjectById
{
   public sealed record GetProjectByIdQuery(Guid Id)
    : IRequest<ProjectDetail?>;
}