using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs;

/// <summary>
/// SignalR hub for real-time notification updates.
/// </summary>
[Authorize]
public sealed class NotificationHub : Hub
{
    /// <summary>
    /// Subscribes the connection to a recipient group.
    /// </summary>
    /// <param name="recipientId">The recipient identifier.</param>
    public Task Subscribe(string recipientId)
    {
        if (string.IsNullOrWhiteSpace(recipientId))
        {
            return Task.CompletedTask;
        }

        return Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(recipientId));
    }

    /// <summary>
    /// Unsubscribes the connection from a recipient group.
    /// </summary>
    /// <param name="recipientId">The recipient identifier.</param>
    public Task Unsubscribe(string recipientId)
    {
        if (string.IsNullOrWhiteSpace(recipientId))
        {
            return Task.CompletedTask;
        }

        return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(recipientId));
    }

    /// <summary>
    /// Gets the group name for a recipient.
    /// </summary>
    public static string GetGroupName(string recipientId)
        => $"recipient:{recipientId}";
}
