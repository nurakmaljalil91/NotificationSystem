using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="NotificationDelivery"/> entity.
/// </summary>
public class NotificationDeliveryConfiguration : IEntityTypeConfiguration<NotificationDelivery>
{
    /// <summary>
    /// Configures the <see cref="NotificationDelivery"/> entity type for Entity Framework.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<NotificationDelivery> builder)
    {
        builder
            .HasOne(x => x.Recipient)
            .WithMany(x => x.Deliveries)
            .HasForeignKey(x => x.RecipientId);
    }
}
