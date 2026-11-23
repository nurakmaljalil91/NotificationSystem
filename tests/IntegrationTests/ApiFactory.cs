#nullable enable
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests;

/// <summary>
/// Web application factory that hosts the API with in-memory infrastructure for integration tests.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Configures the test host with in-memory database, in-memory message broker and test JWT settings.
    /// </summary>
    /// <param name="builder">The web host builder to configure.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("UseInMemoryDatabase", "true");
        builder.UseSetting("MessageBroker:UseInMemory", "true");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();

            var settings = new Dictionary<string, string?>
            {
                ["UseInMemoryDatabase"] = "true",
                ["MessageBroker:UseInMemory"] = "true",
                ["Jwt:Issuer"] = "IntegrationTests",
                ["Jwt:Audience"] = "IntegrationTests",
                ["Jwt:Key"] = "integration-tests-super-secret-key-1234567890",
                ["Jwt:ExpiryMinutes"] = "60"
            };

            config.AddJsonFile("appsettings.json", optional: true);
            config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
            config.AddInMemoryCollection(settings);
        });
    }
}
