using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="NotificationRecipient"/> entity.
/// </summary>
public class NotificationRecipientConfiguration : IEntityTypeConfiguration<NotificationRecipient>
{
    /// <summary>
    /// Configures the <see cref="NotificationRecipient"/> entity type for Entity Framework.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<NotificationRecipient> builder)
    {
        builder
            .Navigation(x => x.Deliveries)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder
            .HasMany(x => x.Deliveries)
            .WithOne(x => x.Recipient)
            .HasForeignKey(x => x.RecipientId);
    }
}
