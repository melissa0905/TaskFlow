using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace TaskFlow.Application.Notifications.Commands.MarkNotificationAsRead
{
    public sealed class MarkNotificationAsReadCommandValidator
    : AbstractValidator<MarkNotificationAsReadCommand>
    {
        public MarkNotificationAsReadCommandValidator()
        {
            RuleFor(x => x.NotificationId)
                .NotEmpty()
                .WithMessage("Bildirim kimliği zorunludur.");

            RuleFor(x => x.RecipientId)
                .NotEmpty()
                .WithMessage("Ekip üyesi seçilmelidir.");
        }
    }
}