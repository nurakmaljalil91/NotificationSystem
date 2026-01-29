namespace Domain.Enums;

/// <summary>
/// Represents the lifecycle state of a notification.
/// </summary>
public enum NotificationStatus
{
    /// <summary>
    /// The notification is pending processing.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// The notification is currently being processed.
    /// </summary>
    Processing = 1,

    /// <summary>
    /// The notification was completed successfully for all channels.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// The notification was partially delivered with some failures.
    /// </summary>
    PartiallyFailed = 3,

    /// <summary>
    /// The notification failed for all channels.
    /// </summary>
    Failed = 4,

    /// <summary>
    /// The notification was cancelled before processing.
    /// </summary>
    Cancelled = 5
}

