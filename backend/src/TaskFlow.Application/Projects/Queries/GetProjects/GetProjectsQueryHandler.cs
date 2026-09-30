using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Abstractions;

namespace TaskFlow.Application.Projects.Queries.GetProjects
{
    public sealed class GetProjectsQueryHandler(
      IProjectRepository projectRepository) 
    : IRequestHandler<GetProjectsQuery, IReadOnlyList<ProjectListItem>>
    {
        public async Task<IReadOnlyList<ProjectListItem>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await projectRepository.GetAllAsync(cancellationToken);

            return projects.Select(p => new ProjectListItem(
                p.Id,
                p.Name,
                p.Description,
                p.CreatedAtUtc)).ToList();
        }
    }
}