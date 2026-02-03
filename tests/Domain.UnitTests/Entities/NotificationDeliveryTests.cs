using Domain.Entities;
using Domain.Enums;

namespace Domain.UnitTests.Entities;

/// <summary>
/// Unit tests for the <see cref="NotificationDelivery"/> entity.
/// </summary>
public class NotificationDeliveryTests
{
    /// <summary>
    /// Verifies default behavior for a new <see cref="NotificationDelivery"/>.
    /// </summary>
    [Fact]
    public void Defaults_AreInitialized()
    {
        var delivery = new NotificationDelivery();

        Assert.Equal(DeliveryStatus.Queued, delivery.Status);
        Assert.Equal(0, delivery.AttemptCount);
    }
}
