using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace TaskFlow.Application.Notifications.Queries.GetNotifications
{
    public sealed class GetNotificationsQueryValidator : AbstractValidator<GetNotificationsQuery>
    {
        public GetNotificationsQueryValidator()
        {
            RuleFor(x => x.RecipientId)
                .NotEmpty()
                .WithMessage("Ekip üyesi seçilmelidir.");

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Sayfa numarası en az 1 olmalıdır.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Sayfa boyutu 1 ile 100 arasında olmalıdır.");
        }
    }
}