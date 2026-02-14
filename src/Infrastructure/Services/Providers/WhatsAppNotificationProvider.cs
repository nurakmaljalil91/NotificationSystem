using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;

namespace Infrastructure.Services.Providers;

/// <summary>
/// Stub WhatsApp provider implementation.
/// </summary>
public sealed class WhatsAppNotificationProvider : INotificationDeliveryProvider
{
    /// <inheritdoc />
    public NotificationChannel Channel => NotificationChannel.WhatsApp;

    /// <inheritdoc />
    public Task<NotificationDeliveryResult> SendAsync(
        NotificationDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        var messageId = $"whatsapp-{Guid.NewGuid():N}";
        return Task.FromResult(NotificationDeliveryResult.Sent("whatsapp-stub", messageId));
    }
}
