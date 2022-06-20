using System.Security.Claims;
using EventStore.Client;
using Microsoft.Extensions.Hosting;

namespace Bookstore.EventStore;

public class EventStoreService : IHostedService
{
    private readonly EventStoreClient _esConnection;
    private readonly IEnumerable<SubscriptionManager> _subscriptionManagers;

    public EventStoreService(
        EventStoreClient esConnection,
        IEnumerable<SubscriptionManager> subscriptionManagers)
    {
        _esConnection = esConnection;
        _subscriptionManagers = subscriptionManagers;
        Principal = new ClaimsPrincipal();
    }

    public ClaimsPrincipal Principal { get; set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(_subscriptionManagers.Select(projection => projection.Start()));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var sm in _subscriptionManagers) sm.Stop();
        await _esConnection.DisposeAsync();
    }
}