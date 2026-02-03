using Domain.Entities;
using Domain.Enums;

namespace Domain.UnitTests.Entities;

/// <summary>
/// Unit tests for the <see cref="Notification"/> entity.
/// </summary>
public class NotificationTests
{
    /// <summary>
    /// Verifies default behavior for a new <see cref="Notification"/>.
    /// </summary>
    [Fact]
    public void Defaults_AreInitialized()
    {
        var notification = new Notification();

        Assert.Equal(NotificationPriority.Normal, notification.Priority);
        Assert.Equal(NotificationStatus.Pending, notification.Status);
        Assert.NotNull(notification.Recipients);
        Assert.Empty(notification.Recipients);
    }

    /// <summary>
    /// Verifies recipients can be tracked on the notification.
    /// </summary>
    [Fact]
    public void Recipients_CanBeAdded()
    {
        var notification = new Notification();
        var recipient = new NotificationRecipient();

        notification.Recipients.Add(recipient);

        Assert.Single(notification.Recipients);
        Assert.Same(recipient, notification.Recipients[0]);
    }
}
