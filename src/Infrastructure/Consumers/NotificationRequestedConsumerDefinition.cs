using MassTransit;

namespace Infrastructure.Consumers;

/// <summary>
/// Configures endpoint settings for <see cref="NotificationRequestedConsumer"/>.
/// </summary>
public class NotificationRequestedConsumerDefinition
    : ConsumerDefinition<NotificationRequestedConsumer>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationRequestedConsumerDefinition"/> class.
    /// </summary>
    public NotificationRequestedConsumerDefinition()
    {
        EndpointName = "notification-requested";
    }
}
