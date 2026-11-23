#nullable enable
using Mediator;

namespace WebAPI.UnitTests.TestInfrastructure;

/// <summary>
/// Mediator stub for controller tests that records the last request and returns a configured response.
/// </summary>
public sealed class TestMediator : IMediator
{
    private readonly Func<object, Task<object>> _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestMediator"/> class.
    /// </summary>
    /// <param name="handler">Produces the response for each sent request.</param>
    public TestMediator(Func<object, Task<object>> handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Gets the most recent request passed to <see cref="Send{TResponse}"/>.
    /// </summary>
    public object? LastRequest { get; private set; }

    /// <summary>
    /// Records the request and returns the configured handler response.
    /// </summary>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The request being sent.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The response produced by the configured handler.</returns>
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        LastRequest = request;
        var result = await _handler(request);
        return (TResponse)result;
    }

    /// <summary>
    /// Ignores published notifications.
    /// </summary>
    /// <param name="notification">The notification being published.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A completed task.</returns>
    public Task Publish(INotification notification, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
