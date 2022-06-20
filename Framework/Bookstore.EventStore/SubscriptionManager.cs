using Bookstore.EventSourcing;
using EventStore.Client;

namespace Bookstore.EventStore;

public class SubscriptionManager
{
    private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<SubscriptionManager>();

    private readonly ICheckpointStore _checkpointStore;
    private readonly EventStoreClient _connection;
    private readonly string _name;
    private readonly ISubscription[] _subscriptions;
    private StreamSubscription? _subscription;

    public SubscriptionManager(
        EventStoreClient connection,
        ICheckpointStore checkpointStore,
        string name,
        params ISubscription[] subscriptions)
    {
        _connection = connection;
        _checkpointStore = checkpointStore;
        _name = name;
        _subscriptions = subscriptions;
    }

    public async Task Start()
    {
        Log.Debug("Starting the projection manager...");
        var position = await _checkpointStore.GetCheckpoint();
        Log.Debug($"Retrieved checkpoint {position}");
        _subscription =
            await _connection.SubscribeToAllAsync(position == null ? FromAll.Start : FromAll.After(GetPosition()), EventAppeared);
        Log.Debug("Subscribed to $all stream");

        Position GetPosition()
        {
            return new Position(position.Value, position.Value);
        }
    }

    public void Stop()
    {
        _subscription?.Dispose();
    }

    private async Task EventAppeared(StreamSubscription _, ResolvedEvent resolvedEvent,
        CancellationToken cancellationToken = default)
    {
        // skip system events
        if (resolvedEvent.Event.EventType.StartsWith("$")) return;
        object? @event;
        try
        {
            @event = resolvedEvent.Deserialize()
                     ?? throw new ArgumentNullException(nameof(resolvedEvent), "null Event Appeared");
        }
        catch (Exception)
        {
            @event = null;
        }

        if (@event == null) return;
        Log.Debug($"Projecting event {@event}");
        try
        {
            await Task.WhenAll(_subscriptions.Select(x => x.Project(@event)));
            await _checkpointStore.StoreCheckpoint(resolvedEvent.OriginalPosition?.CommitPosition);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error occurred while projecting the event {@event}");
        }
    }
}