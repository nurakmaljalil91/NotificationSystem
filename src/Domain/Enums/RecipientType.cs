namespace Domain.Enums;

/// <summary>
/// Specifies the type of recipient for a notification.
/// </summary>
public enum RecipientType
{
    /// <summary>
    /// Recipient type is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Internal staff recipient.
    /// </summary>
    Staff = 1,

    /// <summary>
    /// Customer recipient.
    /// </summary>
    Customer = 2,

    /// <summary>
    /// System or integration recipient.
    /// </summary>
    System = 3
}

