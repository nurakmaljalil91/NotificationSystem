using MassTransit;
using Notification.Contracts.Events;

namespace Infrastructure.Consumers;

/// <summary>
/// Consumes notification requests and persists them for delivery.
/// </summary>
public class NotificationRequestedConsumer : IConsumer<NotificationRequestedV1>
{
    private readonly NotificationRequestedHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationRequestedConsumer"/> class.
    /// </summary>
    public NotificationRequestedConsumer(
        NotificationRequestedHandler handler)
    {
        _handler = handler;
    }

    /// <inheritdoc />
    public async Task Consume(ConsumeContext<NotificationRequestedV1> context)
    {
        await _handler.Handle(context.Message, context.CancellationToken);
    }
}
