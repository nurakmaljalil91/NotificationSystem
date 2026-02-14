using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;

namespace Infrastructure.Services.Providers;

/// <summary>
/// Stub push provider implementation.
/// </summary>
public sealed class PushNotificationProvider : INotificationDeliveryProvider
{
    /// <inheritdoc />
    public NotificationChannel Channel => NotificationChannel.Push;

    /// <inheritdoc />
    public Task<NotificationDeliveryResult> SendAsync(
        NotificationDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        var messageId = $"push-{Guid.NewGuid():N}";
        return Task.FromResult(NotificationDeliveryResult.Sent("push-stub", messageId));
    }
}
