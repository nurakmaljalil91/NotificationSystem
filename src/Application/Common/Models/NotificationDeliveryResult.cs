#nullable enable
namespace Application.Common.Models;

/// <summary>
/// Represents the outcome of a delivery attempt.
/// </summary>
public sealed record NotificationDeliveryResult
{
    /// <summary>
    /// Gets whether the delivery succeeded.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the provider name used for delivery.
    /// </summary>
    public string Provider { get; init; } = string.Empty;

    /// <summary>
    /// Gets the provider message identifier.
    /// </summary>
    public string? ProviderMessageId { get; init; }

    /// <summary>
    /// Gets the failure reason when delivery fails.
    /// </summary>
    public string? FailureReason { get; init; }

    /// <summary>
    /// Creates a successful delivery result.
    /// </summary>
    public static NotificationDeliveryResult Sent(string provider, string? providerMessageId)
        => new()
        {
            Success = true,
            Provider = provider,
            ProviderMessageId = providerMessageId
        };

    /// <summary>
    /// Creates a failed delivery result.
    /// </summary>
    public static NotificationDeliveryResult Failed(string provider, string? reason)
        => new()
        {
            Success = false,
            Provider = provider,
            FailureReason = reason
        };
}
