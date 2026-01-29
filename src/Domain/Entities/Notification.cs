#nullable enable
using System.Collections.Generic;
using NodaTime;

namespace Domain.Entities;

/// <summary>
/// Represents a notification to be delivered to one or more recipients.
/// </summary>
public class Notification : BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the notification title or subject.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the notification body content.
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// Gets or sets the template key used to render this notification.
    /// </summary>
    public string? TemplateKey { get; set; }

    /// <summary>
    /// Gets or sets the template version used to render this notification.
    /// </summary>
    public string? TemplateVersion { get; set; }

    /// <summary>
    /// Gets or sets the service that emitted the source event.
    /// </summary>
    public string? SourceService { get; set; }

    /// <summary>
    /// Gets or sets the type of source event that triggered this notification.
    /// </summary>
    public string? SourceEventType { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the source event to support idempotency.
    /// </summary>
    public string? SourceEventId { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier for tracing across services.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the deep link associated with this notification.
    /// </summary>
    public string? DeepLinkUrl { get; set; }

    /// <summary>
    /// Gets or sets the serialized metadata payload for this notification.
    /// </summary>
    public string? MetadataJson { get; set; }

    /// <summary>
    /// Gets or sets the priority level of this notification.
    /// </summary>
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Gets or sets the current processing status of the notification.
    /// </summary>
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    /// <summary>
    /// Gets or sets the time the notification is scheduled to be processed.
    /// </summary>
    public Instant? ScheduledFor { get; set; }

    /// <summary>
    /// Gets or sets the time the notification processing completed.
    /// </summary>
    public Instant? ProcessedAt { get; set; }

    /// <summary>
    /// Gets or sets the recipients for this notification.
    /// </summary>
    public IList<NotificationRecipient> Recipients { get; private set; } = new List<NotificationRecipient>();
}

