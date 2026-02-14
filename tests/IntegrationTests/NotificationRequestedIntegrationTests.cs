using Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Notification.Contracts.Events;
using Notification.Contracts.Models;

namespace IntegrationTests;

public class NotificationRequestedIntegrationTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public NotificationRequestedIntegrationTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PublishRequest_PersistsNotification()
    {
        using var client = _factory.CreateClient();

        var message = new NotificationRequestedV1
        {
            SourceService = "payments",
            SourceEventType = "PaymentCaptured",
            SourceEventId = "pay-777",
            Title = "Payment captured",
            Body = "Your payment was captured.",
            Channels = new[] { NotificationChannelV1.Email },
            Recipients = new[]
            {
                new NotificationRecipientV1
                {
                    RecipientId = "user-55",
                    Email = "user55@example.com",
                    InAppEnabled = true
                }
            }
        };

        using var publishScope = _factory.Services.CreateScope();
        var publishEndpoint = publishScope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        await publishEndpoint.Publish(message);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var stored = await WaitForNotificationAsync(dbContext, "payments", "pay-777");

        Assert.NotNull(stored);
        Assert.Single(stored!.Recipients);
        Assert.Single(stored.Recipients[0].Deliveries);
    }

    private static async Task<Domain.Entities.Notification?> WaitForNotificationAsync(
        ApplicationDbContext dbContext,
        string sourceService,
        string sourceEventId)
    {
        for (var attempt = 0; attempt < 10; attempt += 1)
        {
            var stored = await dbContext.Notifications
                .Include(notification => notification.Recipients)
                .ThenInclude(recipient => recipient.Deliveries)
                .SingleOrDefaultAsync(
                    notification => notification.SourceService == sourceService
                        && notification.SourceEventId == sourceEventId);

            if (stored != null)
            {
                return stored;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(200));
        }

        return null;
    }
}
