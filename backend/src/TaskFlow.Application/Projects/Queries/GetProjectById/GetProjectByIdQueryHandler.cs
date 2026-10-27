using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Abstractions;

namespace TaskFlow.Application.Projects.Queries.GetProjectById
{
    public sealed class GetProjectByIdQueryHandler(IProjectRepository projectRepository) : IRequestHandler<GetProjectByIdQuery, ProjectDetail>
    {
        public async Task<ProjectDetail> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.GetByIdAsync(request.Id,cancellationToken);

            if(project is null)
            {
                return null;
            }
            return new ProjectDetail(project.Id,project.Name,project.Description);
        }
    }
}