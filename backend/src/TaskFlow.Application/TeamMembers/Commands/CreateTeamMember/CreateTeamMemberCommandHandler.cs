using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.Abstractions;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.TeamMembers.Commands.CreateTeamMember
{
    public class CreateTeamMemberCommandHandler(
        ITeamMemberRepository repository
    ) : IRequestHandler<CreateTeamMemberCommand, Guid>
    {
        public async Task<Guid> Handle(
            CreateTeamMemberCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("Ad soyad boş olamaz.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("E-posta boş olamaz.");

            var email = request.Email.Trim().ToLowerInvariant();

            if (await repository.EmailExistsAsync(email, cancellationToken))
                throw new InvalidOperationException("Bu e-posta zaten kayıtlı.");
            var member = new TeamMember
            {
                FullName = request.FullName.Trim(),
                Email = email
            };
            await repository.AddAsync(member, cancellationToken);
            return member.Id;
        }
    }
}