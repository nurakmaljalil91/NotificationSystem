using Application.Common.Messaging.Events;
using MassTransit;

namespace Infrastructure.Consumers;

/// <summary>
/// Consumes delivery enqueue events and marks deliveries as in progress.
/// </summary>
public class NotificationDeliveryEnqueuedConsumer : IConsumer<NotificationDeliveryEnqueuedV1>
{
    private readonly NotificationDeliveryEnqueuedHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationDeliveryEnqueuedConsumer"/> class.
    /// </summary>
    public NotificationDeliveryEnqueuedConsumer(
        NotificationDeliveryEnqueuedHandler handler)
    {
        _handler = handler;
    }

    /// <inheritdoc />
    public async Task Consume(ConsumeContext<NotificationDeliveryEnqueuedV1> context)
    {
        await _handler.Handle(context.Message.DeliveryId, context.CancellationToken);
    }
}
