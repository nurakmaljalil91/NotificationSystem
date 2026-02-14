using Application.Common.Models;
using Application.Notifications.Commands;
using Application.Notifications.Models;
using Application.Notifications.Queries;
using Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// API controller for in-app notifications.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationsController"/> class.
    /// </summary>
    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves notifications for a recipient.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedEnumerable<NotificationItemDto>>>> GetNotifications(
        [FromQuery] GetNotificationsQuery query)
        => Ok(await _mediator.Send(query));

    /// <summary>
    /// Marks a notification as read for a recipient.
    /// </summary>
    [HttpPost("{notificationId:long}/read")]
    public async Task<ActionResult<BaseResponse<NotificationItemDto>>> MarkRead(
        long notificationId,
        [FromQuery] string recipientId)
    {
        var command = new MarkNotificationReadCommand
        {
            NotificationId = notificationId,
            RecipientId = recipientId
        };

        return Ok(await _mediator.Send(command));
    }
}
