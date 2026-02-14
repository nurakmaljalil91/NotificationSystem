using Application.Common.Interfaces;
using Application.Common.Messaging.Events;
using Infrastructure.Consumers;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Notification.Contracts.Events;
using Notification.Contracts.Models;

namespace Infrastructure.UnitTests.Consumers;

/// <summary>
/// Tests for the NotificationRequestedHandler consumer.
/// </summary>
public class NotificationRequestedConsumerTests
{
    /// <summary>
    /// Verifies that handling a notification request persists the notification to the database
    /// and publishes a delivery enqueued event.
    /// </summary>
    [Fact]
    public async Task Handle_PersistsNotificationAndPublishesDelivery()
    {
        using var provider = BuildServiceProvider();
        using var scope = provider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<NotificationRequestedHandler>();
        var publisher = (FakeNotificationPublisher)scope.ServiceProvider
            .GetRequiredService<INotificationPublisher>();

        var message = new NotificationRequestedV1
        {
            SourceService = "billing",
            SourceEventType = "InvoiceCreated",
            SourceEventId = "inv-001",
            Title = "Invoice ready",
            Body = "Your invoice is ready.",
            Priority = NotificationPriorityV1.Normal,
            Channels = new[] { NotificationChannelV1.Email },
            Recipients = new[]
            {
                new NotificationRecipientV1
                {
                    RecipientId = "user-1",
                    Email = "user@example.com",
                    InAppEnabled = true
                }
            }
        };

        await handler.Handle(message, CancellationToken.None);

        Assert.Single(publisher.Published);
        Assert.IsType<NotificationDeliveryEnqueuedV1>(publisher.Published[0]);

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var stored = await dbContext.Notifications
            .Include(notification => notification.Recipients)
            .ThenInclude(recipient => recipient.Deliveries)
            .SingleOrDefaultAsync(
                notification => notification.SourceService == "billing"
                    && notification.SourceEventId == "inv-001");

        Assert.NotNull(stored);
        Assert.Single(stored!.Recipients);
        Assert.Single(stored.Recipients[0].Deliveries);
    }

    /// <summary>
    /// Verifies that handling the same notification request multiple times is idempotent
    /// and only creates a single notification in the database.
    /// </summary>
    [Fact]
    public async Task Handle_IsIdempotentForSameSourceEvent()
    {
        using var provider = BuildServiceProvider();
        using var scope = provider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<NotificationRequestedHandler>();

        var message = new NotificationRequestedV1
        {
            SourceService = "claims",
            SourceEventType = "ClaimUpdated",
            SourceEventId = "claim-123",
            Title = "Claim updated",
            Recipients = new[]
            {
                new NotificationRecipientV1
                {
                    RecipientId = "user-9",
                    InAppEnabled = true
                }
            }
        };

        await handler.Handle(message, CancellationToken.None);
        await handler.Handle(message, CancellationToken.None);

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var count = await dbContext.Notifications.CountAsync(
            notification => notification.SourceService == "claims"
                && notification.SourceEventId == "claim-123");

        Assert.Equal(1, count);
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"notifications-{Guid.NewGuid()}"));
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddSingleton<IClockService, ClockService>();
        services.AddSingleton<INotificationPublisher, FakeNotificationPublisher>();
        services.AddSingleton<ILogger<NotificationRequestedHandler>>(
            _ => NullLogger<NotificationRequestedHandler>.Instance);
        services.AddTransient<NotificationRequestedHandler>();

        return services.BuildServiceProvider(true);
    }

    private sealed class FakeNotificationPublisher : INotificationPublisher
    {
        public List<NotificationDeliveryEnqueuedV1> Published { get; } = new();

        public Task PublishDeliveryEnqueuedAsync(
            NotificationDeliveryEnqueuedV1 message,
            CancellationToken cancellationToken)
        {
            Published.Add(message);
            return Task.CompletedTask;
        }
    }
}
