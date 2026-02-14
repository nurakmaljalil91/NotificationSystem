using System.Linq;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Data;

/// <summary>
/// Unit tests for notification entity configuration.
/// </summary>
public class NotificationEntityConfigurationTests
{
    /// <summary>
    /// Ensures notification entities and relationships are registered in the model.
    /// </summary>
    [Fact]
    public void Model_RegistersNotificationRelationships()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var notificationEntity = context.Model.FindEntityType(typeof(Domain.Entities.Notification));
        var recipientEntity = context.Model.FindEntityType(typeof(NotificationRecipient));
        var deliveryEntity = context.Model.FindEntityType(typeof(NotificationDelivery));

        Assert.NotNull(notificationEntity);
        Assert.NotNull(recipientEntity);
        Assert.NotNull(deliveryEntity);

        var recipientsNavigation = notificationEntity!.FindNavigation(nameof(Domain.Entities.Notification.Recipients));
        Assert.NotNull(recipientsNavigation);
        Assert.Equal(typeof(NotificationRecipient), recipientsNavigation!.TargetEntityType.ClrType);
        Assert.Equal(
            nameof(NotificationRecipient.NotificationId),
            recipientsNavigation.ForeignKey.Properties.Single().Name);

        var deliveriesNavigation = recipientEntity!.FindNavigation(nameof(NotificationRecipient.Deliveries));
        Assert.NotNull(deliveriesNavigation);
        Assert.Equal(typeof(NotificationDelivery), deliveriesNavigation!.TargetEntityType.ClrType);
        Assert.Equal(
            nameof(NotificationDelivery.RecipientId),
            deliveriesNavigation.ForeignKey.Properties.Single().Name);
    }
}
