using System;
using MassTransit;

namespace Infrastructure.Consumers;

/// <summary>
/// Configures endpoint settings for <see cref="TransactionSucceededConsumer"/>.
/// </summary>
public class TransactionSucceededConsumerDefinition : ConsumerDefinition<TransactionSucceededConsumer>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionSucceededConsumerDefinition"/> class.
    /// </summary>
    public TransactionSucceededConsumerDefinition()
    {
        EndpointName = "transactions-succeeded";
    }

    /// <inheritdoc />
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<TransactionSucceededConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(5)));
    }
}
