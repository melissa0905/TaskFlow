using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskFlow.Contracts.Events
{
    public sealed record NotificationCreated(
     Guid SourceEventId,
     Guid RecipientId);
}