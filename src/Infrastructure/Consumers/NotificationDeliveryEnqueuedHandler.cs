using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Infrastructure.Consumers;

/// <summary>
/// Handles delivery enqueue events by invoking providers and updating status.
/// </summary>
public sealed class NotificationDeliveryEnqueuedHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IClockService _clockService;
    private readonly INotificationDeliveryProviderResolver _providerResolver;
    private readonly ILogger<NotificationDeliveryEnqueuedHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationDeliveryEnqueuedHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="clockService">The clock service for timestamps.</param>
    /// <param name="providerResolver">The provider resolver.</param>
    /// <param name="logger">The logger instance.</param>
    public NotificationDeliveryEnqueuedHandler(
        IApplicationDbContext context,
        IClockService clockService,
        INotificationDeliveryProviderResolver providerResolver,
        ILogger<NotificationDeliveryEnqueuedHandler> logger)
    {
        _context = context;
        _clockService = clockService;
        _providerResolver = providerResolver;
        _logger = logger;
    }

    /// <summary>
    /// Handles the delivery enqueue request.
    /// </summary>
    /// <param name="deliveryId">The delivery identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task Handle(long deliveryId, CancellationToken cancellationToken)
    {
        var delivery = await _context.NotificationDeliveries
            .Include(item => item.Recipient)
            .ThenInclude(item => item.Notification)
            .FirstOrDefaultAsync(item => item.Id == deliveryId, cancellationToken);

        if (delivery is null)
        {
            _logger.LogWarning("Notification delivery {DeliveryId} not found.", deliveryId);
            return;
        }

        delivery.Status = DeliveryStatus.Sending;
        delivery.AttemptCount += 1;
        delivery.LastAttemptedAt = _clockService.Now;

        var provider = _providerResolver.GetProvider(delivery.Channel);
        if (provider is null)
        {
            delivery.Status = DeliveryStatus.Failed;
            delivery.FailedAt = _clockService.Now;
            delivery.FailureReason = "No provider registered for channel.";
            await _context.SaveChangesAsync(cancellationToken);
            return;
        }

        var request = new NotificationDeliveryRequest(
            delivery.Recipient.Notification,
            delivery.Recipient,
            delivery);

        var result = await provider.SendAsync(request, cancellationToken);

        if (result.Success)
        {
            delivery.Status = DeliveryStatus.Sent;
            delivery.SentAt = _clockService.Now;
            delivery.Provider = result.Provider;
            delivery.ProviderMessageId = result.ProviderMessageId;
            delivery.FailureReason = null;
        }
        else
        {
            delivery.Status = DeliveryStatus.Failed;
            delivery.FailedAt = _clockService.Now;
            delivery.Provider = result.Provider;
            delivery.FailureReason = result.FailureReason;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
