using Application.Common.Models;
using Domain.Enums;

namespace Application.Common.Interfaces;

/// <summary>
/// Represents a provider capable of delivering notifications for a channel.
/// </summary>
public interface INotificationDeliveryProvider
{
    /// <summary>
    /// Gets the channel this provider supports.
    /// </summary>
    NotificationChannel Channel { get; }

    /// <summary>
    /// Sends a notification delivery request.
    /// </summary>
    /// <param name="request">The delivery request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<NotificationDeliveryResult> SendAsync(
        NotificationDeliveryRequest request,
        CancellationToken cancellationToken);
}
