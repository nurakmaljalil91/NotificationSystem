#nullable enable
using Domain.Enums;

namespace Application.Common.Interfaces;

/// <summary>
/// Resolves notification delivery providers for a channel.
/// </summary>
public interface INotificationDeliveryProviderResolver
{
    /// <summary>
    /// Gets the provider for a channel, or null when none is registered.
    /// </summary>
    INotificationDeliveryProvider? GetProvider(NotificationChannel channel);
}
