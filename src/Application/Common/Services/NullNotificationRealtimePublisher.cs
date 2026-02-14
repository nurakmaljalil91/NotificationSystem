using Application.Common.Interfaces;
using Application.Notifications.Models;

namespace Application.Common.Services;

/// <summary>
/// No-op realtime publisher used when SignalR is not configured.
/// </summary>
public sealed class NullNotificationRealtimePublisher : INotificationRealtimePublisher
{
    /// <inheritdoc />
    public Task PublishNotificationCreatedAsync(
        string recipientId,
        NotificationItemDto notification,
        CancellationToken cancellationToken)
        => Task.CompletedTask;

    /// <inheritdoc />
    public Task PublishNotificationReadAsync(
        string recipientId,
        long notificationId,
        DateTimeOffset readAtUtc,
        CancellationToken cancellationToken)
        => Task.CompletedTask;
}
