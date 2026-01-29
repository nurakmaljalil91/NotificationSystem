#nullable enable
using NodaTime;

namespace Domain.Entities;

/// <summary>
/// Represents a delivery attempt of a notification for a specific channel.
/// </summary>
public class NotificationDelivery : BaseEntity
{
    /// <summary>
    /// Gets or sets the recipient identifier for this delivery attempt.
    /// </summary>
    public long RecipientId { get; set; }

    /// <summary>
    /// Gets or sets the channel used to deliver the notification.
    /// </summary>
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Gets or sets the current delivery status.
    /// </summary>
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Queued;

    /// <summary>
    /// Gets or sets the number of delivery attempts.
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// Gets or sets the last time an attempt was made.
    /// </summary>
    public Instant? LastAttemptedAt { get; set; }

    /// <summary>
    /// Gets or sets the time the delivery was successfully sent.
    /// </summary>
    public Instant? SentAt { get; set; }

    /// <summary>
    /// Gets or sets the time the delivery failed.
    /// </summary>
    public Instant? FailedAt { get; set; }

    /// <summary>
    /// Gets or sets the next scheduled attempt time.
    /// </summary>
    public Instant? NextAttemptAt { get; set; }

    /// <summary>
    /// Gets or sets the provider name used for delivery.
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// Gets or sets the provider message identifier for tracing.
    /// </summary>
    public string? ProviderMessageId { get; set; }

    /// <summary>
    /// Gets or sets the failure reason if delivery failed.
    /// </summary>
    public string? FailureReason { get; set; }

    /// <summary>
    /// Gets or sets the serialized payload sent to the provider.
    /// </summary>
    public string? PayloadJson { get; set; }

    /// <summary>
    /// Gets or sets an external reference ID if required.
    /// </summary>
    public string? ExternalReference { get; set; }

    /// <summary>
    /// Gets or sets the owning recipient for this delivery.
    /// </summary>
    public NotificationRecipient Recipient { get; set; } = null!;
}

