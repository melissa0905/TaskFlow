using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Notifications.Commands.MarkNotificationAsRead;
using TaskFlow.Application.Notifications.Queries.GetNotifications;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
   public sealed class NotificationsController(ISender sender)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid recipientId,
        [FromQuery] bool onlyUnread = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetNotificationsQuery(
                recipientId,
                onlyUnread,
                page,
                pageSize),
            cancellationToken);

        return Ok(result);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(
        Guid id,
        [FromQuery] Guid recipientId,
        CancellationToken cancellationToken)
    {
        var found = await sender.Send(
            new MarkNotificationAsReadCommand(
                id,
                recipientId),
            cancellationToken);

        if (!found)
        {
            return Problem(
                title: "Bildirim bulunamadı.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return NoContent();
    }
}
}