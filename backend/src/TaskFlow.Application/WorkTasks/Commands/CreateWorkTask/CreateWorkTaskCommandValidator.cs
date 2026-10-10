using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace TaskFlow.Application.WorkTasks.Commands.CreateWorkTask
{
    public sealed class CreateWorkTaskCommandValidator : AbstractValidator<CreateWorkTaskCommand>
    {
        public CreateWorkTaskCommandValidator()
        {
            RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Proje seçilmelidir.");

            RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Başlık boş olamaz.")
            .MaximumLength(200)
            .WithMessage("Başlık en fazla 200 karakter olabilir.");

            RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Görev açıklaması boş olamaz.")
            .MaximumLength(2000)
            .WithMessage("Açıklama en fazla 2000 karakter olabilir.");
            RuleFor(x => x.DueDate)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty()
                 .WithMessage("Son tarih zorunludur.")
                 .Must(date => date.Kind == DateTimeKind.Utc)
                 .WithMessage("Son tarih UTC formatında gönderilmelidir.");

            RuleFor(x => x.AssignedToId)
                .Must(id => !id.HasValue || id.Value != Guid.Empty)
                .WithMessage("Geçerli bir ekip üyesi seçilmelidir.");

        }

    }
}
