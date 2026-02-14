using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;

namespace Infrastructure.Services.Providers;

/// <summary>
/// Stub email provider implementation.
/// </summary>
public sealed class EmailNotificationProvider : INotificationDeliveryProvider
{
    /// <inheritdoc />
    public NotificationChannel Channel => NotificationChannel.Email;

    /// <inheritdoc />
    public Task<NotificationDeliveryResult> SendAsync(
        NotificationDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        var messageId = $"email-{Guid.NewGuid():N}";
        return Task.FromResult(NotificationDeliveryResult.Sent("email-stub", messageId));
    }
}
