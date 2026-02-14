using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Notifications.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Notifications.Commands;

/// <summary>
/// Marks a notification as read for a recipient.
/// </summary>
public sealed class MarkNotificationReadCommand : IRequest<BaseResponse<NotificationItemDto>>
{
    /// <summary>
    /// Gets or sets the notification identifier.
    /// </summary>
    public long NotificationId { get; set; }

    /// <summary>
    /// Gets or sets the recipient identifier.
    /// </summary>
    public string RecipientId { get; set; } = string.Empty;
}

/// <summary>
/// Handles marking notifications as read.
/// </summary>
public sealed class MarkNotificationReadCommandHandler
    : IRequestHandler<MarkNotificationReadCommand, BaseResponse<NotificationItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IClockService _clockService;
    private readonly INotificationRealtimePublisher _realtimePublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkNotificationReadCommandHandler"/> class.
    /// </summary>
    public MarkNotificationReadCommandHandler(
        IApplicationDbContext context,
        IClockService clockService,
        INotificationRealtimePublisher realtimePublisher)
    {
        _context = context;
        _clockService = clockService;
        _realtimePublisher = realtimePublisher;
    }

    /// <inheritdoc />
    public async Task<BaseResponse<NotificationItemDto>> Handle(
        MarkNotificationReadCommand request,
        CancellationToken cancellationToken)
    {
        var recipientId = request.RecipientId?.Trim();
        if (string.IsNullOrWhiteSpace(recipientId))
        {
            return BaseResponse<NotificationItemDto>.Fail("RecipientId is required.");
        }

        var recipient = await _context.NotificationRecipients
            .Include(item => item.Notification)
            .FirstOrDefaultAsync(
                item => item.NotificationId == request.NotificationId
                    && item.RecipientId == recipientId,
                cancellationToken);

        if (recipient is null)
        {
            throw new NotFoundException("Notification recipient not found.");
        }

        if (!recipient.IsRead)
        {
            recipient.IsRead = true;
            recipient.ReadAt = _clockService.Now;
            await _context.SaveChangesAsync(cancellationToken);

            await _realtimePublisher.PublishNotificationReadAsync(
                recipientId,
                request.NotificationId,
                recipient.ReadAt?.ToDateTimeOffset() ?? DateTimeOffset.UtcNow,
                cancellationToken);
        }

        var dto = NotificationItemDto.FromEntities(recipient.Notification, recipient);
        return BaseResponse<NotificationItemDto>.Ok(dto, "Notification marked as read.");
    }
}
