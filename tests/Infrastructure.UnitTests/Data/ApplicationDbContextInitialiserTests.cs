using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Infrastructure.UnitTests.Data;

/// <summary>
/// Unit tests for <see cref="ApplicationDbContextInitialiser"/>.
/// </summary>
public class ApplicationDbContextInitialiserTests
{
    /// <summary>
    /// Ensures the seed method adds default data when the database is empty.
    /// </summary>
    [Fact]
    public async Task TrySeedAsync_AddsDefaultDataWhenEmpty()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);
        var logger = NullLogger<ApplicationDbContextInitialiser>.Instance;
        var initialiser = new ApplicationDbContextInitialiser(logger, context);

        await initialiser.TrySeedAsync();

        var notification = await context.Notifications
            .Include(item => item.Recipients)
            .ThenInclude(item => item.Deliveries)
            .SingleOrDefaultAsync();

        Assert.NotNull(notification);
        Assert.Single(notification!.Recipients);
        Assert.Single(notification.Recipients[0].Deliveries);
    }
}
