using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Application.Common.Interfaces;
using Application.Common.Messaging.Events;
using Application.Common.Messaging.Models;
using Infrastructure.Consumers;
using Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.UnitTests.Consumers;

/// <summary>
/// Unit tests for <see cref="TransactionSucceededConsumer"/>.
/// </summary>
public class TransactionSucceededConsumerTests
{
    /// <summary>
    /// Ensures transaction events persist notifications and enqueue delivery jobs.
    /// </summary>
    [Fact]
    public async Task Consume_PersistsNotificationAndEnqueuesDeliveries()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        await using var provider = services.BuildServiceProvider(true);
        using var scope = provider.CreateScope();
        var publishEndpoint = new FakePublishEndpoint();
        var logger = NullLogger<TransactionSucceededConsumer>.Instance;
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var consumer = new TransactionSucceededConsumer(dbContext, publishEndpoint, logger);

        await consumer.HandleMessage(new TransactionSucceededV1
        {
            EventId = Guid.NewGuid().ToString(),
            TransactionId = "txn-123",
            UserId = "user-1",
            Amount = 125.50m,
            Currency = "USD",
            Recipients = new[]
            {
                new NotificationRecipientV1
                {
                    RecipientId = "user-1",
                    Email = "user@example.com",
                    InAppEnabled = true
                }
            }
        }, CancellationToken.None);

        var notification = await dbContext.Notifications
            .Include(item => item.Recipients)
            .ThenInclude(item => item.Deliveries)
            .SingleAsync();

        Assert.Equal(1, notification.Recipients.Count);
        Assert.Equal(2, notification.Recipients.Single().Deliveries.Count);
        Assert.Equal(2, publishEndpoint.PublishedNotifications.Count);
    }

    private sealed class FakePublishEndpoint : IPublishEndpoint
    {
        public List<NotificationDeliveryEnqueuedV1> PublishedNotifications { get; } = new();

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer) => throw new NotImplementedException();

        public Task Publish<T>(T message, CancellationToken cancellationToken = default)
            where T : class
        {
            if (message is NotificationDeliveryEnqueuedV1 notification)
            {
                PublishedNotifications.Add(notification);
            }

            return Task.CompletedTask;
        }

        public Task Publish<T>(
            T message,
            IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return Publish(message, cancellationToken);
        }

        public Task Publish<T>(
            T message,
            IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return Publish(message, cancellationToken);
        }

        public Task Publish(object message, CancellationToken cancellationToken = default)
        {
            if (message is NotificationDeliveryEnqueuedV1 notification)
            {
                PublishedNotifications.Add(notification);
            }

            return Task.CompletedTask;
        }

        public Task Publish(
            object message,
            Type messageType,
            CancellationToken cancellationToken = default)
        {
            return Publish(message, cancellationToken);
        }

        public Task Publish(
            object message,
            IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken = default)
        {
            return Publish(message, cancellationToken);
        }

        public Task Publish(
            object message,
            Type messageType,
            IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken = default)
        {
            return Publish(message, cancellationToken);
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken = default)
            where T : class
        {
            return Task.CompletedTask;
        }

        public Task Publish<T>(
            object values,
            IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return Task.CompletedTask;
        }

        public Task Publish<T>(
            object values,
            IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return Task.CompletedTask;
        }
    }

}
