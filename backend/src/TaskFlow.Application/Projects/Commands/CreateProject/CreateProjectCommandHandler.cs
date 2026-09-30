using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Abstractions;
using TaskFlow.Application.Projects.Commands.CreateProject;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Projects.Commands.CreateProject
{
    public sealed class CreateProjectCommandHandler(IProjectRepository projectRepository) : IRequestHandler<CreateProjectCommand, Guid>
    {
        public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Proje adı boş olamaz.");
            }
            
            var project = new Project
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty
            };

            await projectRepository.AddAsync(project, cancellationToken);

            return project.Id;
        }
    }
}