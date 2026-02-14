using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

/// <summary>
/// Represents the application's database context, providing access to application entities.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Gets the set of <see cref="Notification"/> entities.
    /// </summary>
    DbSet<Notification> Notifications { get; }

    /// <summary>
    /// Gets the set of <see cref="NotificationRecipient"/> entities.
    /// </summary>
    DbSet<NotificationRecipient> NotificationRecipients { get; }

    /// <summary>
    /// Gets the set of <see cref="NotificationDelivery"/> entities.
    /// </summary>
    DbSet<NotificationDelivery> NotificationDeliveries { get; }

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
