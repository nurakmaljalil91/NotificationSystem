using Application.Common.Interfaces;
using Application.Common.Messaging.Events;
using Domain.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Consumers;

/// <summary>
/// Consumes delivery enqueue events and marks deliveries as in progress.
/// </summary>
public class NotificationDeliveryEnqueuedConsumer : IConsumer<NotificationDeliveryEnqueuedV1>
{
    private readonly IApplicationDbContext _context;
    private readonly IClockService _clockService;
    private readonly ILogger<NotificationDeliveryEnqueuedConsumer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationDeliveryEnqueuedConsumer"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="clockService">The clock service for timestamps.</param>
    /// <param name="logger">The logger instance.</param>
    public NotificationDeliveryEnqueuedConsumer(
        IApplicationDbContext context,
        IClockService clockService,
        ILogger<NotificationDeliveryEnqueuedConsumer> logger)
    {
        _context = context;
        _clockService = clockService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Consume(ConsumeContext<NotificationDeliveryEnqueuedV1> context)
    {
        var delivery = await _context.NotificationDeliveries
            .FirstOrDefaultAsync(
                item => item.Id == context.Message.DeliveryId,
                context.CancellationToken);

        if (delivery is null)
        {
            _logger.LogWarning(
                "Notification delivery {DeliveryId} not found.",
                context.Message.DeliveryId);
            return;
        }

        delivery.Status = DeliveryStatus.Sending;
        delivery.AttemptCount += 1;
        delivery.LastAttemptedAt = _clockService.Now;

        await _context.SaveChangesAsync(context.CancellationToken);
    }
}
