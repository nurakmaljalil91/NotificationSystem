using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.TestInfrastructure;

/// <summary>
/// Represents a test implementation of <see cref="IApplicationDbContext"/> using Entity Framework Core for unit testing purposes.
/// </summary>
public sealed class TestApplicationDbContext : DbContext, IApplicationDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestApplicationDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public TestApplicationDbContext(DbContextOptions<TestApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the <see cref="DbSet{TodoList}"/> representing the collection of todo lists.
    /// </summary>
    public DbSet<TodoList> TodoLists => Set<TodoList>();

    /// <summary>
    /// Gets the <see cref="DbSet{TodoItem}"/> representing the collection of todo items.
    /// </summary>
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    /// <summary>
    /// Gets the <see cref="DbSet{Notification}"/> representing the collection of notifications.
    /// </summary>
    public DbSet<Notification> Notifications => Set<Notification>();

    /// <summary>
    /// Gets the <see cref="DbSet{NotificationRecipient}"/> representing the collection of notification recipients.
    /// </summary>
    public DbSet<NotificationRecipient> NotificationRecipients => Set<NotificationRecipient>();

    /// <summary>
    /// Gets the <see cref="DbSet{NotificationDelivery}"/> representing the collection of notification deliveries.
    /// </summary>
    public DbSet<NotificationDelivery> NotificationDeliveries => Set<NotificationDelivery>();

    /// <summary>
    /// Configures the entity mappings for the context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoList>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.OwnsOne(x => x.Colour);
            builder.Navigation(x => x.Items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<TodoItem>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.List)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.ListId);
        });

        modelBuilder.Entity<Notification>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Navigation(x => x.Recipients)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.HasMany(x => x.Recipients)
                .WithOne(x => x.Notification)
                .HasForeignKey(x => x.NotificationId);
        });

        modelBuilder.Entity<NotificationRecipient>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Navigation(x => x.Deliveries)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.HasMany(x => x.Deliveries)
                .WithOne(x => x.Recipient)
                .HasForeignKey(x => x.RecipientId);
        });

        modelBuilder.Entity<NotificationDelivery>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Recipient)
                .WithMany(x => x.Deliveries)
                .HasForeignKey(x => x.RecipientId);
        });
    }
}
