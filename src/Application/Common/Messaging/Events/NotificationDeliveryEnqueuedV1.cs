using System;
using Domain.Enums;

namespace Application.Common.Messaging.Events;

/// <summary>
/// Represents a delivery job queued for processing.
/// </summary>
public record NotificationDeliveryEnqueuedV1
{
    /// <summary>
    /// Gets the notification identifier.
    /// </summary>
    public long NotificationId { get; init; }

    /// <summary>
    /// Gets the recipient identifier.
    /// </summary>
    public long RecipientId { get; init; }

    /// <summary>
    /// Gets the delivery identifier.
    /// </summary>
    public long DeliveryId { get; init; }

    /// <summary>
    /// Gets the channel used for delivery.
    /// </summary>
    public NotificationChannel Channel { get; init; }

    /// <summary>
    /// Gets the time this job was enqueued.
    /// </summary>
    public DateTimeOffset EnqueuedAt { get; init; } = DateTimeOffset.UtcNow;
}
