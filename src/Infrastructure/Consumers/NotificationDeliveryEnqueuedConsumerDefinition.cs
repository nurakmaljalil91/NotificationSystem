using System;
using MassTransit;

namespace Infrastructure.Consumers;

/// <summary>
/// Configures endpoint settings for <see cref="NotificationDeliveryEnqueuedConsumer"/>.
/// </summary>
public class NotificationDeliveryEnqueuedConsumerDefinition
    : ConsumerDefinition<NotificationDeliveryEnqueuedConsumer>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationDeliveryEnqueuedConsumerDefinition"/> class.
    /// </summary>
    public NotificationDeliveryEnqueuedConsumerDefinition()
    {
        EndpointName = "notification-delivery-enqueued";
    }

    /// <inheritdoc />
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<NotificationDeliveryEnqueuedConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(retry => retry.Interval(5, TimeSpan.FromSeconds(10)));
    }
}
