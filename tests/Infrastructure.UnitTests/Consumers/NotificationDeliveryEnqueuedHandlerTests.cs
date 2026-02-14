using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Consumers;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

#nullable enable

namespace Infrastructure.UnitTests.Consumers;

/// <summary>
/// Contains unit tests for the NotificationDeliveryEnqueuedHandler class, verifying the notification delivery process
/// when enqueued.  
/// </summary>
/// <remarks>This test class ensures that notifications are sent correctly and that the delivery status is updated
/// as expected. It includes tests for successful delivery, provider failures, and cases where no provider is available.
/// Each test uses an in-memory database to isolate test scenarios and ensure consistent results.</remarks>
public class NotificationDeliveryEnqueuedHandlerTests
{
    /// <summary>
    /// Verifies that a notification delivery is successfully sent and marked with Sent status when a provider is available.
    /// </summary>
    [Fact]
    public async Task Handle_SendsAndMarksDeliveryAsSent()
    {
        using var provider = BuildServiceProvider(new FakeProvider(NotificationChannel.Email));
        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var deliveryId = await SeedDeliveryAsync(dbContext, NotificationChannel.Email);

        var handler = scope.ServiceProvider.GetRequiredService<NotificationDeliveryEnqueuedHandler>();
        await handler.Handle(deliveryId, CancellationToken.None);

        var delivery = await dbContext.NotificationDeliveries.FindAsync(deliveryId);

        Assert.NotNull(delivery);
        Assert.Equal(DeliveryStatus.Sent, delivery!.Status);
        Assert.NotNull(delivery.SentAt);
        Assert.NotNull(delivery.LastAttemptedAt);
        Assert.Equal(1, delivery.AttemptCount);
        Assert.Equal("email-stub", delivery.Provider);
        Assert.NotNull(delivery.ProviderMessageId);
        Assert.Null(delivery.FailureReason);
    }

    /// <summary>
    /// Verifies that a notification delivery is marked as failed when the provider encounters an error during sending.
    /// </summary>
    [Fact]
    public async Task Handle_ProviderFailureMarksDeliveryAsFailed()
    {
        using var provider = BuildServiceProvider(new FakeProvider(NotificationChannel.Sms, shouldFail: true));
        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var deliveryId = await SeedDeliveryAsync(dbContext, NotificationChannel.Sms);

        var handler = scope.ServiceProvider.GetRequiredService<NotificationDeliveryEnqueuedHandler>();
        await handler.Handle(deliveryId, CancellationToken.None);

        var delivery = await dbContext.NotificationDeliveries.FindAsync(deliveryId);

        Assert.NotNull(delivery);
        Assert.Equal(DeliveryStatus.Failed, delivery!.Status);
        Assert.NotNull(delivery.FailedAt);
        Assert.Equal("sms-stub", delivery.Provider);
        Assert.Equal("stub failure", delivery.FailureReason);
    }

    /// <summary>
    /// Verifies that a notification delivery is marked as failed when no provider is registered for the channel.
    /// </summary>
    [Fact]
    public async Task Handle_NoProviderMarksDeliveryAsFailed()
    {
        using var provider = BuildServiceProvider(provider: null);
        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var deliveryId = await SeedDeliveryAsync(dbContext, NotificationChannel.Push);

        var handler = scope.ServiceProvider.GetRequiredService<NotificationDeliveryEnqueuedHandler>();
        await handler.Handle(deliveryId, CancellationToken.None);

        var delivery = await dbContext.NotificationDeliveries.FindAsync(deliveryId);

        Assert.NotNull(delivery);
        Assert.Equal(DeliveryStatus.Failed, delivery!.Status);
        Assert.Equal("No provider registered for channel.", delivery.FailureReason);
    }

    private static ServiceProvider BuildServiceProvider(FakeProvider? provider)
    {
        var services = new ServiceCollection();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"delivery-{Guid.NewGuid()}"));
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddSingleton<IClockService, ClockService>();
        services.AddSingleton<INotificationDeliveryProviderResolver>(
            _ => new FakeResolver(provider));
        services.AddSingleton<ILogger<NotificationDeliveryEnqueuedHandler>>(
            _ => NullLogger<NotificationDeliveryEnqueuedHandler>.Instance);
        services.AddTransient<NotificationDeliveryEnqueuedHandler>();

        return services.BuildServiceProvider(true);
    }

    private static async Task<long> SeedDeliveryAsync(
        ApplicationDbContext dbContext,
        NotificationChannel channel)
    {
        var notification = new Domain.Entities.Notification
        {
            Title = "Test",
            Body = "Body"
        };

        var recipient = new NotificationRecipient
        {
            RecipientId = "user-1",
            RecipientType = RecipientType.Individual,
            Notification = notification
        };

        var delivery = new NotificationDelivery
        {
            Channel = channel,
            Status = DeliveryStatus.Queued,
            Recipient = recipient
        };

        recipient.Deliveries.Add(delivery);
        notification.Recipients.Add(recipient);

        dbContext.Notifications.Add(notification);
        await dbContext.SaveChangesAsync();

        return delivery.Id;
    }

    private sealed class FakeResolver : INotificationDeliveryProviderResolver
    {
        private readonly INotificationDeliveryProvider? _provider;

        public FakeResolver(INotificationDeliveryProvider? provider)
        {
            _provider = provider;
        }

        public INotificationDeliveryProvider? GetProvider(NotificationChannel channel)
        {
            if (_provider is null)
            {
                return null;
            }

            return _provider.Channel == channel ? _provider : null;
        }
    }

    private sealed class FakeProvider : INotificationDeliveryProvider
    {
        private readonly bool _shouldFail;

        public FakeProvider(NotificationChannel channel, bool shouldFail = false)
        {
            Channel = channel;
            _shouldFail = shouldFail;
        }

        public NotificationChannel Channel { get; }

        public Task<NotificationDeliveryResult> SendAsync(
            NotificationDeliveryRequest request,
            CancellationToken cancellationToken)
        {
            if (_shouldFail)
            {
                return Task.FromResult(NotificationDeliveryResult.Failed($"{Channel.ToString().ToLowerInvariant()}-stub", "stub failure"));
            }

            return Task.FromResult(NotificationDeliveryResult.Sent($"{Channel.ToString().ToLowerInvariant()}-stub", Guid.NewGuid().ToString("N")));
        }
    }
}
