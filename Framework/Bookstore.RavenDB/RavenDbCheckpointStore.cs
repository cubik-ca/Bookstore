using Bookstore.EventSourcing;
using Bookstore.EventStore;
using EventStore.Client;
using Raven.Client.Documents.Session;

namespace Bookstore.RavenDB;

public class RavenDbCheckpointStore : ICheckpointStore
{
    private readonly IAsyncDocumentSession _session;
    private readonly string _checkpointName;

    public RavenDbCheckpointStore(IAsyncDocumentSession session, string checkpointName)
    {
        _session = session;
        _checkpointName = checkpointName;
    }

    public async Task<ulong?> GetCheckpoint()
    {
        var checkpoint = await _session.LoadAsync<Checkpoint>($"checkpoints/{_checkpointName}");
        return checkpoint?.Position ?? Position.Start.CommitPosition;
    }

    public async Task StoreCheckpoint(ulong? position)
    {
        Checkpoint? checkpoint = null;
        if (position != null)
            checkpoint = await _session.LoadAsync<Checkpoint>($"checkpoints/{_checkpointName}");
        if (checkpoint == null)
        {
            checkpoint = new Checkpoint { Id = _checkpointName };
            await _session.StoreAsync(checkpoint);
        }
        checkpoint.Position = position;
        await _session.SaveChangesAsync();
    }
}