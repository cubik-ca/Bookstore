using System.Security.Claims;
using Bookstore.EventSourcing;
using Bookstore.EventStore;
using EventStore.Client;

namespace Bookstore.EventStore;

public class FunctionalStore : IFunctionalAggregateStore
{
    private readonly EventStoreClient _connection;

    public FunctionalStore(EventStoreClient connection)
    {
        _connection = connection;
    }

    public Task<T> Load<T>(Guid id) where T : IAggregateState<T>, new()
    {
        return Load<T>(id, (x, e) => x.When(x, e));
    }

    private async Task<T> Load<T>(Guid id, Func<T, object, T> when)
        where T : IAggregateState<T>, new()
    {
        var state = new T();
        var streamName = state.GetStreamName(id);
        ulong? position = 0L;
        var events = new List<object>();
        var slice = _connection.ReadStreamAsync(Direction.Forwards,
            streamName,
            StreamPosition.FromInt64(Convert.ToInt64(position)));
        events.AddRange(await slice.Select(x => x.Deserialize()).ToArrayAsync());
        return events.Aggregate(state, when);
    }

    Task IFunctionalAggregateStore.Save<T>(long version, AggregateState<T>.Result update)
    {
        return _connection.AppendEvents(Thread.CurrentPrincipal as ClaimsPrincipal, update.State.StreamName, version,
            update.Events.ToArray());
    }
}