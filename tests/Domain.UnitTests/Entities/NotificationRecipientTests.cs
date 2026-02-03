using Domain.Entities;
using Domain.Enums;

namespace Domain.UnitTests.Entities;

/// <summary>
/// Unit tests for the <see cref="NotificationRecipient"/> entity.
/// </summary>
public class NotificationRecipientTests
{
    /// <summary>
    /// Verifies default behavior for a new <see cref="NotificationRecipient"/>.
    /// </summary>
    [Fact]
    public void Defaults_AreInitialized()
    {
        var recipient = new NotificationRecipient();

        Assert.Equal(RecipientType.Unknown, recipient.RecipientType);
        Assert.True(recipient.InAppEnabled);
        Assert.False(recipient.IsRead);
        Assert.NotNull(recipient.Deliveries);
        Assert.Empty(recipient.Deliveries);
    }

    /// <summary>
    /// Verifies deliveries can be tracked on the recipient.
    /// </summary>
    [Fact]
    public void Deliveries_CanBeAdded()
    {
        var recipient = new NotificationRecipient();
        var delivery = new NotificationDelivery();

        recipient.Deliveries.Add(delivery);

        Assert.Single(recipient.Deliveries);
        Assert.Same(delivery, recipient.Deliveries[0]);
    }
}
