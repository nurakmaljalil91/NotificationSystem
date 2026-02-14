using Application.Common.Interfaces;
using Application.Notifications.Models;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Hubs;

namespace WebAPI.Services;

/// <summary>
/// Publishes real-time notification updates via SignalR.
/// </summary>
public sealed class SignalRNotificationRealtimePublisher : INotificationRealtimePublisher
{
    private readonly IHubContext<NotificationHub> _hubContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignalRNotificationRealtimePublisher"/> class.
    /// </summary>
    public SignalRNotificationRealtimePublisher(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <inheritdoc />
    public Task PublishNotificationCreatedAsync(
        string recipientId,
        NotificationItemDto notification,
        CancellationToken cancellationToken)
    {
        return _hubContext.Clients
            .Group(NotificationHub.GetGroupName(recipientId))
            .SendAsync("notificationReceived", notification, cancellationToken);
    }

    /// <inheritdoc />
    public Task PublishNotificationReadAsync(
        string recipientId,
        long notificationId,
        DateTimeOffset readAtUtc,
        CancellationToken cancellationToken)
    {
        return _hubContext.Clients
            .Group(NotificationHub.GetGroupName(recipientId))
            .SendAsync("notificationRead", new { notificationId, readAtUtc }, cancellationToken);
    }
}
