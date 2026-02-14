using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;

namespace Infrastructure.Services.Providers;

/// <summary>
/// Stub SMS provider implementation.
/// </summary>
public sealed class SmsNotificationProvider : INotificationDeliveryProvider
{
    /// <inheritdoc />
    public NotificationChannel Channel => NotificationChannel.Sms;

    /// <inheritdoc />
    public Task<NotificationDeliveryResult> SendAsync(
        NotificationDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        var messageId = $"sms-{Guid.NewGuid():N}";
        return Task.FromResult(NotificationDeliveryResult.Sent("sms-stub", messageId));
    }
}
