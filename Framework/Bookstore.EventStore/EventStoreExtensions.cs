using System.Security.Claims;
using System.Text;
using Bookstore.EventSourcing;
using EventStore.Client;
using Newtonsoft.Json;

namespace Bookstore.EventStore;

public static class EventStoreExtensions
{
    private static byte[] Serialize(object data)
    {
        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
    }

    public static Task AppendEvents(
        this EventStoreClient connection,
        ClaimsPrincipal? principal,
        string streamName,
        long version,
        params object[] events)
    {
        var user = principal?.Identity?.Name;
        if (!events.Any()) return Task.CompletedTask;
        var preparedEvents = events
            .Select(
                @event =>
                    new EventData(
                        Uuid.FromGuid(Guid.NewGuid()),
                        TypeMapper.GetTypeName(@event.GetType()) ?? throw new Exception("Type name not found"),
                        Serialize(@event),
                        Serialize(new EventMetadata { User = user, ClrType = @event.GetType().FullName }))).ToArray();
        return connection.AppendToStreamAsync(streamName, StreamRevision.FromInt64(version), preparedEvents);
    }
}