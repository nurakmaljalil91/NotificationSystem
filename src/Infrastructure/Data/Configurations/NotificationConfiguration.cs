using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="Notification"/> entity.
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    /// <summary>
    /// Configures the <see cref="Notification"/> entity type for Entity Framework.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder
            .Navigation(x => x.Recipients)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder
            .HasMany(x => x.Recipients)
            .WithOne(x => x.Notification)
            .HasForeignKey(x => x.NotificationId);
    }
}
