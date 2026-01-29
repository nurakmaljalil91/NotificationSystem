namespace Domain.Enums;

/// <summary>
/// Represents the delivery status for a specific notification channel.
/// </summary>
public enum DeliveryStatus
{
    /// <summary>
    /// The delivery is queued.
    /// </summary>
    Queued = 0,

    /// <summary>
    /// The delivery is in progress.
    /// </summary>
    Sending = 1,

    /// <summary>
    /// The delivery was sent successfully.
    /// </summary>
    Sent = 2,

    /// <summary>
    /// The delivery failed.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// The delivery is scheduled for retry.
    /// </summary>
    Retrying = 4,

    /// <summary>
    /// The delivery was skipped.
    /// </summary>
    Skipped = 5
}

