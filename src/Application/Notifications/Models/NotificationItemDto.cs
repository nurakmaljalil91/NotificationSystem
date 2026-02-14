using Domain.Entities;
using Domain.Enums;

namespace Application.Notifications.Models;

/// <summary>
/// Represents a notification item for client consumption.
/// </summary>
public sealed record NotificationItemDto
{
    /// <summary>
    /// Gets the notification identifier.
    /// </summary>
    public long NotificationId { get; init; }

    /// <summary>
    /// Gets the recipient identifier.
    /// </summary>
    public string RecipientId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the notification title.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Gets the notification body.
    /// </summary>
    public string? Body { get; init; }

    /// <summary>
    /// Gets the deep link URL.
    /// </summary>
    public string? DeepLinkUrl { get; init; }

    /// <summary>
    /// Gets the metadata payload as JSON.
    /// </summary>
    public string? MetadataJson { get; init; }

    /// <summary>
    /// Gets the notification priority.
    /// </summary>
    public NotificationPriority Priority { get; init; }

    /// <summary>
    /// Gets the notification status.
    /// </summary>
    public NotificationStatus Status { get; init; }

    /// <summary>
    /// Gets the source service name.
    /// </summary>
    public string? SourceService { get; init; }

    /// <summary>
    /// Gets the source event type.
    /// </summary>
    public string? SourceEventType { get; init; }

    /// <summary>
    /// Gets whether the notification is read.
    /// </summary>
    public bool IsRead { get; init; }

    /// <summary>
    /// Gets the time the notification was read.
    /// </summary>
    public DateTimeOffset? ReadAtUtc { get; init; }

    /// <summary>
    /// Gets the time the notification was created.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; init; }

    /// <summary>
    /// Creates a DTO from entity data.
    /// </summary>
    public static NotificationItemDto FromEntities(Notification notification, NotificationRecipient recipient)
        => new()
        {
            NotificationId = notification.Id,
            RecipientId = recipient.RecipientId ?? string.Empty,
            Title = notification.Title,
            Body = notification.Body,
            DeepLinkUrl = notification.DeepLinkUrl,
            MetadataJson = notification.MetadataJson,
            Priority = notification.Priority,
            Status = notification.Status,
            SourceService = notification.SourceService,
            SourceEventType = notification.SourceEventType,
            IsRead = recipient.IsRead,
            ReadAtUtc = recipient.ReadAt?.ToDateTimeOffset(),
            CreatedAtUtc = notification.CreatedDate.ToDateTimeOffset()
        };
}
