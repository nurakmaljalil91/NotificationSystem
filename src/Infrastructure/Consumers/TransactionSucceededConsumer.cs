using System.Linq;
using Application.Common.Interfaces;
using Application.Common.Messaging.Events;
using Domain.Entities;
using Domain.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Consumers;

/// <summary>
/// Consumes transaction success events and persists notifications.
/// </summary>
public class TransactionSucceededConsumer : IConsumer<TransactionSucceededV1>
{
    private readonly IApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<TransactionSucceededConsumer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionSucceededConsumer"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="publishEndpoint">The publish endpoint for enqueueing deliveries.</param>
    /// <param name="logger">The logger instance.</param>
    public TransactionSucceededConsumer(
        IApplicationDbContext context,
        IPublishEndpoint publishEndpoint,
        ILogger<TransactionSucceededConsumer> logger)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Consume(ConsumeContext<TransactionSucceededV1> context)
    {
        await HandleMessage(context.Message, context.CancellationToken);
    }

    /// <summary>
    /// Handles a transaction success message and persists a notification.
    /// </summary>
    /// <param name="message">The transaction message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task HandleMessage(
        TransactionSucceededV1 message,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(message.EventId))
        {
            _logger.LogWarning("TransactionSucceededV1 missing EventId; skipping.");
            return;
        }

        var exists = await _context.Notifications
            .AsNoTracking()
            .AnyAsync(
                notification => notification.SourceEventId == message.EventId
                    && notification.SourceEventType == nameof(TransactionSucceededV1),
                cancellationToken);

        if (exists)
        {
            _logger.LogInformation(
                "TransactionSucceededV1 with EventId {EventId} already processed.",
                message.EventId);
            return;
        }

        var notification = new Notification
        {
            Title = message.Title ?? "Transfer completed",
            Body = message.Body ?? $"Your transfer of {message.Amount} {message.Currency} was successful.",
            TemplateKey = message.TemplateKey,
            TemplateVersion = message.TemplateVersion,
            SourceService = message.SourceService,
            SourceEventType = nameof(TransactionSucceededV1),
            SourceEventId = message.EventId,
            CorrelationId = message.CorrelationId,
            MetadataJson = message.MetadataJson,
            Priority = message.Priority,
            Status = NotificationStatus.Pending
        };

        foreach (var recipient in message.Recipients)
        {
            var recipientEntity = new NotificationRecipient
            {
                RecipientId = recipient.RecipientId,
                RecipientType = RecipientType.Individual,
                DisplayName = recipient.DisplayName,
                Email = recipient.Email,
                PhoneNumber = recipient.PhoneNumber,
                WhatsAppNumber = recipient.WhatsAppNumber,
                PushToken = recipient.PushToken,
                InAppEnabled = recipient.InAppEnabled
            };

            AddDeliveryIfSet(recipientEntity, recipient.Email, NotificationChannel.Email);
            AddDeliveryIfSet(recipientEntity, recipient.PhoneNumber, NotificationChannel.Sms);
            AddDeliveryIfSet(recipientEntity, recipient.WhatsAppNumber, NotificationChannel.WhatsApp);
            AddDeliveryIfSet(recipientEntity, recipient.PushToken, NotificationChannel.Push);

            if (recipient.InAppEnabled)
            {
                recipientEntity.Deliveries.Add(new NotificationDelivery
                {
                    Channel = NotificationChannel.InApp,
                    Status = DeliveryStatus.Queued
                });
            }

            notification.Recipients.Add(recipientEntity);
        }

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);

        var deliveryJobs = notification.Recipients
            .SelectMany(recipient => recipient.Deliveries.Select(delivery => new NotificationDeliveryEnqueuedV1
            {
                NotificationId = notification.Id,
                RecipientId = recipient.Id,
                DeliveryId = delivery.Id,
                Channel = delivery.Channel
            }))
            .ToList();

        foreach (var job in deliveryJobs)
        {
            await _publishEndpoint.Publish(job, cancellationToken);
        }
    }

    private static void AddDeliveryIfSet(
        NotificationRecipient recipient,
        string? value,
        NotificationChannel channel)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        recipient.Deliveries.Add(new NotificationDelivery
        {
            Channel = channel,
            Status = DeliveryStatus.Queued
        });
    }
}
