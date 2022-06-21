using System.Reflection;
using Bookstore.EventSourcing;
using EventStore.Client;
using Serilog;

namespace Bookstore.EventStore;

public class EsAggregateStore : IAggregateStore
{
    private static readonly ILogger Log = Serilog.Log.ForContext<EsAggregateStore>();
    private readonly EventStoreClient _connection;
    private readonly EventStoreService _service;

    public EsAggregateStore(EventStoreClient connection, EventStoreService service)
    {
        _connection = connection;
        _service = service;
    }

    public async Task<bool> Exists<T>(AggregateId<T> aggregateId) where T : AggregateRoot<T>
    {
        var stream = GetStreamName(aggregateId);
        Log.Debug($"Checking existence of stream {aggregateId}");
        var result = _connection.ReadStreamAsync(Direction.Forwards, stream, StreamPosition.Start);
        return await result.ReadState != ReadState.StreamNotFound;
    }

    public async Task<T> Load<T>(AggregateId<T> aggregateId) where T : AggregateRoot<T>
    {
        if (aggregateId == null) throw new ArgumentNullException(nameof(aggregateId));
        var stream = GetStreamName(aggregateId);
        var aggregate = (T?) Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.NonPublic, null,
                            new object[] {aggregateId}, null)
                        ?? throw new Exception($"Could not create instance of {typeof(T).Name}");
        var page = _connection.ReadStreamAsync(Direction.Forwards, stream, StreamPosition.Start);
        Log.Debug($"Loading events for the aggregate {aggregate}");
        aggregate.Load(await page.Select(resolvedEvent => resolvedEvent.Deserialize()).ToArrayAsync());
        return aggregate;
    }

    public async Task Save<T>(T aggregate) where T : AggregateRoot<T>
    {
        if (aggregate == null)
            throw new ArgumentNullException(nameof(aggregate));
        var streamName = GetStreamName(aggregate);
        var changes = aggregate.GetChanges().ToArray();
        foreach (var change in changes)
            Log.Debug($"Persisting event {change}");
        await _connection.AppendEvents(_service.Principal, streamName, aggregate.Version, changes);
        aggregate.ClearChanges();
    }

    private static string GetStreamName<T>(AggregateId<T> aggregateId) where T : AggregateRoot<T>
    {
        return $"{typeof(T).Name}-{aggregateId}";
    }

    private static string GetStreamName<T>(T aggregate) where T : AggregateRoot<T>
    {
        return $"{typeof(T).Name}-{aggregate.Id}";
    }
}