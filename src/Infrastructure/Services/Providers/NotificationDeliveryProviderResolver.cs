#nullable enable
using Application.Common.Interfaces;
using Domain.Enums;

namespace Infrastructure.Services.Providers;

/// <summary>
/// Resolves delivery providers from registered services.
/// </summary>
public sealed class NotificationDeliveryProviderResolver : INotificationDeliveryProviderResolver
{
    private readonly IReadOnlyDictionary<NotificationChannel, INotificationDeliveryProvider> _providers;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationDeliveryProviderResolver"/> class.
    /// </summary>
    /// <param name="providers">The registered providers.</param>
    public NotificationDeliveryProviderResolver(IEnumerable<INotificationDeliveryProvider> providers)
    {
        _providers = providers.ToDictionary(provider => provider.Channel);
    }

    /// <inheritdoc />
    public INotificationDeliveryProvider? GetProvider(NotificationChannel channel)
        => _providers.TryGetValue(channel, out var provider) ? provider : null;
}
