using System;
using System.Collections.Generic;
using Application.Common.Messaging.Models;
using Domain.Enums;

namespace Application.Common.Messaging.Events;

/// <summary>
/// Represents a successful transaction event for notification processing.
/// </summary>
public record TransactionSucceededV1
{
    /// <summary>
    /// Gets the event identifier for idempotency.
    /// </summary>
    public string EventId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the transaction identifier from the source system.
    /// </summary>
    public string TransactionId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the identifier of the user who initiated the transaction.
    /// </summary>
    public string UserId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the transaction amount.
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Gets the currency code for the transaction.
    /// </summary>
    public string Currency { get; init; } = "USD";

    /// <summary>
    /// Gets the event timestamp.
    /// </summary>
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the notification title.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Gets the notification body content.
    /// </summary>
    public string? Body { get; init; }

    /// <summary>
    /// Gets the template key used for rendering.
    /// </summary>
    public string? TemplateKey { get; init; }

    /// <summary>
    /// Gets the template version used for rendering.
    /// </summary>
    public string? TemplateVersion { get; init; }

    /// <summary>
    /// Gets the source service that emitted the event.
    /// </summary>
    public string? SourceService { get; init; }

    /// <summary>
    /// Gets the correlation identifier for tracing.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Gets the serialized metadata payload.
    /// </summary>
    public string? MetadataJson { get; init; }

    /// <summary>
    /// Gets the notification priority.
    /// </summary>
    public NotificationPriority Priority { get; init; } = NotificationPriority.Normal;

    /// <summary>
    /// Gets the recipients for this notification.
    /// </summary>
    public IReadOnlyCollection<NotificationRecipientV1> Recipients { get; init; }
        = Array.Empty<NotificationRecipientV1>();
}
