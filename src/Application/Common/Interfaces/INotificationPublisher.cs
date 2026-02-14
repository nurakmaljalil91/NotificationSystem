using Application.Common.Messaging.Events;

namespace Application.Common.Interfaces;

/// <summary>
/// Publishes notification-related events.
/// </summary>
public interface INotificationPublisher
{
    /// <summary>
    /// Publishes a delivery enqueue event.
    /// </summary>
    /// <param name="message">The delivery event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task PublishDeliveryEnqueuedAsync(
        NotificationDeliveryEnqueuedV1 message,
        CancellationToken cancellationToken);
}
