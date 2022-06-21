using Bookstore.EventSourcing;
using Bookstore.EventStore;
using EventStore.Client;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Bookstore.MongoDB;

public class CheckpointStore : ICheckpointStore
{
    private readonly string _checkpointName;
    private readonly IMongoClient _mongodb;

    public CheckpointStore(IMongoClient mongodb, string checkpointName)
    {
        _mongodb = mongodb;
        _checkpointName = checkpointName;
    }

    public async Task<ulong?> GetCheckpoint()
    {
        var checkpoints = _mongodb.GetDatabase("Bookstore").GetCollection<Checkpoint>("checkpoints");
        var checkpoint = await checkpoints.AsQueryable().FirstOrDefaultAsync(c => c.Id == _checkpointName);
        return checkpoint?.Position ?? Position.Start.CommitPosition;
    }

    public async Task StoreCheckpoint(ulong? position)
    {
        var checkpoints = _mongodb.GetDatabase("Bookstore").GetCollection<Checkpoint>("checkpoints");
        var checkpoint = await checkpoints.AsQueryable().SingleOrDefaultAsync(c => c.Id == _checkpointName);
        checkpoint ??= new Checkpoint {Id = _checkpointName};
        checkpoint.Position = position;
        await checkpoints.ReplaceOneAsync(c => c.Id == _checkpointName, checkpoint,
            new ReplaceOptions {IsUpsert = true});
    }
}