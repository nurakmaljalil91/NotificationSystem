#nullable enable
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Domain.Common;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests;

/// <summary>
/// Base class for API integration tests that provides HTTP client and response helpers.
/// </summary>
public abstract class ApiTestBase
{
    /// <summary>
    /// Gets the JSON serializer options used to read API responses.
    /// </summary>
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ApiFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiTestBase"/> class.
    /// </summary>
    /// <param name="factory">The factory that hosts the API under test.</param>
    protected ApiTestBase(ApiFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Creates an unauthenticated HTTP client for the API under test.
    /// </summary>
    /// <returns>An HTTP client targeting the test server.</returns>
    protected HttpClient CreateClient()
        => _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });

    /// <summary>
    /// Creates an HTTP client carrying a bearer token for an integration test user.
    /// </summary>
    /// <returns>An authenticated HTTP client targeting the test server.</returns>
    protected async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = CreateClient();
        var token = await GetTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    /// <summary>
    /// Reads and deserializes a JSON response body, asserting that it is not null.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="response">The HTTP response to read.</param>
    /// <returns>The deserialized response payload.</returns>
    protected static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        Assert.NotNull(payload);
        return payload!;
    }

    private static async Task<string> GetTokenAsync(HttpClient client)
    {
        var login = new
        {
            Username = "integration-user",
            Email = "integration@example.com"
        };

        var response = await client.PostAsJsonAsync("/api/Auth/login", login);
        response.EnsureSuccessStatusCode();

        var payload = await ReadResponseAsync<BaseResponse<AuthTokenResponse>>(response);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Data);
        Assert.False(string.IsNullOrWhiteSpace(payload.Data!.Token));

        return payload.Data!.Token;
    }

    /// <summary>
    /// Represents the token payload returned by the login endpoint.
    /// </summary>
    /// <param name="Token">The issued access token.</param>
    /// <param name="ExpiresAtUtc">The UTC time at which the token expires.</param>
    protected sealed record AuthTokenResponse(string Token, DateTime ExpiresAtUtc);

    /// <summary>
    /// Represents a paginated API response.
    /// </summary>
    /// <typeparam name="T">The type of the items in the page.</typeparam>
    protected sealed class PaginatedResponse<T>
    {
        /// <summary>
        /// Gets or sets the items in the current page.
        /// </summary>
        public IEnumerable<T>? Items { get; set; }
        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public int TotalPages { get; set; }
        /// <summary>
        /// Gets or sets the total number of items.
        /// </summary>
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// Represents a to-do item returned by the API.
    /// </summary>
    protected sealed class TodoItemResponse
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the identifier of the list that owns the item.
        /// </summary>
        public long ListId { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the item is done.
        /// </summary>
        public bool Done { get; set; }
    }

    /// <summary>
    /// Represents a to-do list returned by the API.
    /// </summary>
    protected sealed class TodoListResponse
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// Gets or sets the list colour.
        /// </summary>
        public string? Colour { get; set; }
        /// <summary>
        /// Gets or sets the items in the list.
        /// </summary>
        public IReadOnlyCollection<TodoItemResponse>? Items { get; set; }
    }
}
