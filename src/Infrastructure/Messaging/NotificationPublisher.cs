using Application.Common.Interfaces;
using Application.Common.Messaging.Events;
using MassTransit;

namespace Infrastructure.Messaging;

/// <summary>
/// Publishes notification events via MassTransit.
/// </summary>
public class NotificationPublisher : INotificationPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationPublisher"/> class.
    /// </summary>
    /// <param name="publishEndpoint">The MassTransit publish endpoint.</param>
    public NotificationPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    /// <inheritdoc />
    public Task PublishDeliveryEnqueuedAsync(
        NotificationDeliveryEnqueuedV1 message,
        CancellationToken cancellationToken)
    {
        return _publishEndpoint.Publish(message, cancellationToken);
    }
}
