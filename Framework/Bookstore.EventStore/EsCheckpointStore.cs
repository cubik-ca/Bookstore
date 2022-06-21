using System.Text;
using Bookstore.EventSourcing;
using EventStore.Client;
using Newtonsoft.Json;

namespace Bookstore.EventStore;

public class EsCheckpointStore : ICheckpointStore
{
    private const string CheckpointStreamPrefix = "checkpoint:";
    private readonly EventStoreClient _connection;
    private readonly UserCredentials _credentials;
    private readonly string _streamName;

    public EsCheckpointStore(
        EventStoreClient connection,
        UserCredentials credentials,
        string subscriptionName)
    {
        _connection = connection;
        _credentials = credentials;
        _streamName = CheckpointStreamPrefix + subscriptionName;
    }

    public async Task<ulong?> GetCheckpoint()
    {
        var slice = _connection.ReadStreamAsync(Direction.Backwards, _streamName, StreamPosition.End, 1,
            userCredentials: _credentials);
        var eventData = await slice.FirstOrDefaultAsync();
        if (eventData.Equals(default(ResolvedEvent)))
        {
            await StoreCheckpoint(Position.Start.CommitPosition);
            await SetStreamMaxCount();
            return null;
        }

        return eventData.Deserialize<Checkpoint>().Position;
    }

    public Task StoreCheckpoint(ulong? position)
    {
        var @event = new Checkpoint {Position = position};
        var preparedEvent = new EventData(
            Uuid.FromGuid(Guid.NewGuid()),
            "$checkpoint",
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)));
        return _connection.AppendToStreamAsync(_streamName, StreamRevision.None, new[] {preparedEvent},
            userCredentials: _credentials);
    }

    private async Task SetStreamMaxCount()
    {
        var metadata = await _connection.GetStreamMetadataAsync(_streamName);
        if (!metadata.Metadata.MaxCount.HasValue)
            await _connection.SetStreamMetadataAsync(_streamName, StreamRevision.None, new StreamMetadata(),
                userCredentials: _credentials);
    }
}