using Domain.Entities;

namespace Application.Common.Models;

/// <summary>
/// Represents a request to deliver a notification via a provider.
/// </summary>
public sealed record NotificationDeliveryRequest(
    Notification Notification,
    NotificationRecipient Recipient,
    NotificationDelivery Delivery);
