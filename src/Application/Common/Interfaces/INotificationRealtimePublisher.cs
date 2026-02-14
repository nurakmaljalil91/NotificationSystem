using Application.Notifications.Models;

namespace Application.Common.Interfaces;

/// <summary>
/// Publishes real-time notification updates to connected clients.
/// </summary>
public interface INotificationRealtimePublisher
{
    /// <summary>
    /// Publishes a newly created notification.
    /// </summary>
    /// <param name="recipientId">The recipient identifier.</param>
    /// <param name="notification">The notification payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task PublishNotificationCreatedAsync(
        string recipientId,
        NotificationItemDto notification,
        CancellationToken cancellationToken);

    /// <summary>
    /// Publishes a notification read update.
    /// </summary>
    /// <param name="recipientId">The recipient identifier.</param>
    /// <param name="notificationId">The notification identifier.</param>
    /// <param name="readAtUtc">The read timestamp.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task PublishNotificationReadAsync(
        string recipientId,
        long notificationId,
        DateTimeOffset readAtUtc,
        CancellationToken cancellationToken);
}
