#nullable enable
using System.Collections.Generic;
using NodaTime;

namespace Domain.Entities;

/// <summary>
/// Represents a recipient of a notification and their delivery preferences/status.
/// </summary>
public class NotificationRecipient : BaseEntity
{
    /// <summary>
    /// Gets or sets the notification identifier.
    /// </summary>
    public long NotificationId { get; set; }

    /// <summary>
    /// Gets or sets the recipient identifier from the source system.
    /// </summary>
    public string? RecipientId { get; set; }

    /// <summary>
    /// Gets or sets the type of recipient.
    /// </summary>
    public RecipientType RecipientType { get; set; } = RecipientType.Unknown;

    /// <summary>
    /// Gets or sets the display name of the recipient.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the email address for email notifications.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the phone number for SMS notifications.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the WhatsApp number for WhatsApp notifications.
    /// </summary>
    public string? WhatsAppNumber { get; set; }

    /// <summary>
    /// Gets or sets the push token for push notifications.
    /// </summary>
    public string? PushToken { get; set; }

    /// <summary>
    /// Gets or sets whether in-app notifications are enabled for this recipient.
    /// </summary>
    public bool InAppEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the in-app notification was read.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Gets or sets the time the in-app notification was read.
    /// </summary>
    public Instant? ReadAt { get; set; }

    /// <summary>
    /// Gets or sets the parent notification.
    /// </summary>
    public Notification Notification { get; set; } = null!;

    /// <summary>
    /// Gets or sets the delivery attempts for this recipient.
    /// </summary>
    public IList<NotificationDelivery> Deliveries { get; private set; } = new List<NotificationDelivery>();
}

