using System.Text;
using Bookstore.EventSourcing;
using EventStore.Client;
using Newtonsoft.Json;

namespace Bookstore.EventStore;

public static class EventDeserializer
{
    public static object Deserialize(this ResolvedEvent resolvedEvent)
    {
        var dataType = TypeMapper.GetType(resolvedEvent.Event.EventType)
                       ?? throw new ArgumentNullException(nameof(resolvedEvent),
                           $"Data type for {resolvedEvent.Event.EventType} is null");
        var jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
        var data = JsonConvert.DeserializeObject(jsonData, dataType);
        return data ?? throw new InvalidOperationException($"Failed to deserialize event '{jsonData}'");
    }

    public static T Deserialize<T>(this ResolvedEvent resolvedEvent)
    {
        var jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
        return JsonConvert.DeserializeObject<T>(jsonData)
               ?? throw new InvalidOperationException(
                   $"Failed to deserialize event '{jsonData}' to {typeof(T).FullName}");
    }
}