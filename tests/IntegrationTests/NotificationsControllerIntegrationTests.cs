#nullable enable
using System.Net.Http.Json;
using Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Notification.Contracts.Events;
using Notification.Contracts.Models;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the notifications controller endpoints.
/// </summary>
public sealed class NotificationsControllerIntegrationTests : ApiTestBase, IClassFixture<ApiFactory>
{
    private const string RecipientId = "user-42";
    private readonly ApiFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationsControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory fixture.</param>
    public NotificationsControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Tests the notifications flow by publishing a notification, retrieving it, and marking it as read.
    /// </summary>
    [Fact]
    public async Task Notifications_Flow_ReturnsAndMarksRead()
    {
        await PublishNotificationAsync();

        var client = await CreateAuthenticatedClientAsync();
        var listPayload = await WaitForNotificationsAsync(client);
        Assert.True(listPayload.Success);
        Assert.NotNull(listPayload.Data);
        Assert.NotEmpty(listPayload.Data!.Items ?? Array.Empty<NotificationResponse>());

        var notification = listPayload.Data.Items!.First();

        var readResponse = await client.PostAsync(
            $"/api/notifications/{notification.NotificationId}/read?recipientId={RecipientId}",
            JsonContent.Create(new { }));
        readResponse.EnsureSuccessStatusCode();

        var readPayload = await ReadResponseAsync<BaseResponse<NotificationResponse>>(readResponse);
        Assert.True(readPayload.Success);
        Assert.True(readPayload.Data?.IsRead);
        Assert.NotNull(readPayload.Data?.ReadAtUtc);
    }

    private async Task PublishNotificationAsync()
    {
        using var publishScope = _factory.Services.CreateScope();
        var publishEndpoint = publishScope.ServiceProvider.GetRequiredService<MassTransit.IPublishEndpoint>();

        var message = new NotificationRequestedV1
        {
            SourceService = "integration",
            SourceEventType = "NotificationRequested",
            SourceEventId = $"notif-{Guid.NewGuid():N}",
            Title = "Hello",
            Body = "Integration test",
            Channels = new[] { NotificationChannelV1.Email },
            Recipients = new[]
            {
                new NotificationRecipientV1
                {
                    RecipientId = RecipientId,
                    Email = "user42@example.com",
                    InAppEnabled = true
                }
            }
        };

        await publishEndpoint.Publish(message);
    }

    private static async Task<BaseResponse<PaginatedResponse<NotificationResponse>>> WaitForNotificationsAsync(HttpClient client)
    {
        for (var attempt = 0; attempt < 10; attempt += 1)
        {
            var listResponse = await client.GetAsync($"/api/notifications?recipientId={RecipientId}");
            listResponse.EnsureSuccessStatusCode();

            var listPayload = await ReadResponseAsync<BaseResponse<PaginatedResponse<NotificationResponse>>>(listResponse);

            if (listPayload.Data?.Items?.Any() == true)
            {
                return listPayload;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(200));
        }

        var finalResponse = await client.GetAsync($"/api/notifications?recipientId={RecipientId}");
        finalResponse.EnsureSuccessStatusCode();
        return await ReadResponseAsync<BaseResponse<PaginatedResponse<NotificationResponse>>>(finalResponse);
    }

    private sealed class NotificationResponse
    {
#pragma warning disable S3459, S1144 // Properties are assigned via JSON deserialization
        public long NotificationId { get; init; }
        public bool IsRead { get; init; }
        public DateTimeOffset? ReadAtUtc { get; init; }
#pragma warning restore S3459, S1144
    }
}
