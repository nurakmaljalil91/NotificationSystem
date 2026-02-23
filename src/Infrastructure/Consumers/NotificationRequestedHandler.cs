using Application.Common.Interfaces;
using Application.Common.Messaging.Events;
using Application.Notifications.Models;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notification.Contracts.Events;
using Notification.Contracts.Models;
using NodaTime;

namespace Infrastructure.Consumers;

/// <summary>
/// Handles notification requests by persisting and enqueueing deliveries.
/// </summary>
public class NotificationRequestedHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IClockService _clockService;
    private readonly INotificationPublisher _notificationPublisher;
    private readonly INotificationRealtimePublisher _realtimePublisher;
    private readonly ILogger<NotificationRequestedHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationRequestedHandler"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="clockService">The clock service for timestamps.</param>
    /// <param name="notificationPublisher">The publisher for follow-up events.</param>
    /// <param name="realtimePublisher">The realtime publisher for in-app notifications.</param>
    /// <param name="logger">The logger instance.</param>
    public NotificationRequestedHandler(
        IApplicationDbContext context,
        IClockService clockService,
        INotificationPublisher notificationPublisher,
        INotificationRealtimePublisher realtimePublisher,
        ILogger<NotificationRequestedHandler> logger)
    {
        _context = context;
        _clockService = clockService;
        _notificationPublisher = notificationPublisher;
        _realtimePublisher = realtimePublisher;
        _logger = logger;
    }

    /// <summary>
    /// Handles the incoming notification request.
    /// </summary>
    /// <param name="message">The notification request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task Handle(NotificationRequestedV1 message, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(message.SourceService)
            && !string.IsNullOrWhiteSpace(message.SourceEventId))
        {
            var exists = await _context.Notifications.AnyAsync(
                notification => notification.SourceService == message.SourceService
                    && notification.SourceEventId == message.SourceEventId,
                cancellationToken);

            if (exists)
            {
                _logger.LogInformation(
                    "Notification request {SourceService}:{SourceEventId} already processed.",
                    message.SourceService,
                    message.SourceEventId);
                return;
            }
        }
        else
        {
            _logger.LogWarning(
                "Notification request missing idempotency fields (SourceService/SourceEventId).");
        }

        var notification = new Domain.Entities.Notification
        {
            Title = message.Title,
            Body = message.Body,
            TemplateKey = message.TemplateKey,
            TemplateVersion = message.TemplateVersion,
            SourceService = message.SourceService,
            SourceEventType = message.SourceEventType,
            SourceEventId = message.SourceEventId,
            CorrelationId = message.CorrelationId,
            DeepLinkUrl = message.DeepLinkUrl,
            MetadataJson = message.MetadataJson,
            Priority = MapPriority(message.Priority),
            ScheduledFor = message.ScheduledForUtc.HasValue
                ? Instant.FromDateTimeOffset(message.ScheduledForUtc.Value)
                : null
        };

        foreach (var recipient in message.Recipients)
        {
            var recipientEntity = new NotificationRecipient
            {
                RecipientId = recipient.RecipientId,
                RecipientType = MapRecipientType(recipient.RecipientType),
                DisplayName = recipient.DisplayName,
                Email = recipient.Email,
                PhoneNumber = recipient.PhoneNumber,
                WhatsAppNumber = recipient.WhatsAppNumber,
                PushToken = recipient.PushToken,
                InAppEnabled = recipient.InAppEnabled,
                Notification = notification
            };

            var channels = ResolveChannels(recipient, message);

            foreach (var channel in channels)
            {
                if (channel == NotificationChannelV1.InApp)
                {
                    continue;
                }

                var delivery = new NotificationDelivery
                {
                    Channel = MapChannel(channel),
                    Status = DeliveryStatus.Queued,
                    AttemptCount = 0,
                    LastAttemptedAt = null,
                    Recipient = recipientEntity
                };

                recipientEntity.Deliveries.Add(delivery);
            }

            notification.Recipients.Add(recipientEntity);
        }

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var delivery in notification.Recipients.SelectMany(item => item.Deliveries))
        {
            await _notificationPublisher.PublishDeliveryEnqueuedAsync(
                new NotificationDeliveryEnqueuedV1
                {
                    NotificationId = notification.Id,
                    RecipientId = delivery.RecipientId,
                    DeliveryId = delivery.Id,
                    Channel = delivery.Channel,
                    EnqueuedAt = _clockService.Now.ToDateTimeOffset()
                },
                cancellationToken);
        }

        foreach (var recipient in notification.Recipients)
        {
            if (!recipient.InAppEnabled || string.IsNullOrWhiteSpace(recipient.RecipientId))
            {
                continue;
            }

            var payload = NotificationItemDto.FromEntities(notification, recipient);
            await _realtimePublisher.PublishNotificationCreatedAsync(
                recipient.RecipientId!,
                payload,
                cancellationToken);
        }
    }

    private static IReadOnlyCollection<NotificationChannelV1> ResolveChannels(
        NotificationRecipientV1 recipient,
        NotificationRequestedV1 message)
    {
        if (recipient.Channels.Count > 0)
        {
            return recipient.Channels;
        }

        if (message.Channels.Count > 0)
        {
            return message.Channels;
        }

        return recipient.InAppEnabled
            ? new[] { NotificationChannelV1.InApp }
            : Array.Empty<NotificationChannelV1>();
    }

    private static NotificationPriority MapPriority(NotificationPriorityV1 priority)
        => priority switch
        {
            NotificationPriorityV1.Low => NotificationPriority.Low,
            NotificationPriorityV1.Normal => NotificationPriority.Normal,
            NotificationPriorityV1.High => NotificationPriority.High,
            NotificationPriorityV1.Urgent => NotificationPriority.Urgent,
            _ => NotificationPriority.None
        };

    private static NotificationChannel MapChannel(NotificationChannelV1 channel)
        => channel switch
        {
            NotificationChannelV1.Email => NotificationChannel.Email,
            NotificationChannelV1.Sms => NotificationChannel.Sms,
            NotificationChannelV1.WhatsApp => NotificationChannel.WhatsApp,
            NotificationChannelV1.Push => NotificationChannel.Push,
            _ => NotificationChannel.InApp
        };

    private static RecipientType MapRecipientType(RecipientTypeV1 recipientType)
        => recipientType switch
        {
            RecipientTypeV1.Individual => RecipientType.Individual,
            RecipientTypeV1.Organization => RecipientType.Organization,
            RecipientTypeV1.Group => RecipientType.Group,
            RecipientTypeV1.System => RecipientType.System,
            _ => RecipientType.Unknown
        };
}
