using Bookstore.EventSourcing;
using Raven.Client.Documents.Session;

namespace Bookstore.RavenDB;

public class RavenDbProjection<T> : ISubscription
{
    private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<RavenDbProjection<T>>();
    private static readonly string ReadModelName = typeof(T).Name;

    private IAsyncDocumentSession _session;
    private readonly Projector<T> _projector;

    public RavenDbProjection(
        IAsyncDocumentSession session,
        Projector<T> projector)
    {
        _session = session;
        _projector = projector;
    }

    public async Task Project(object @event)
    {
        var handler = _projector.Invoke(_session, @event);
        if (handler == null) return;
        Log.Debug($"Projecting {@event} to {ReadModelName}");
        await handler();
    }
}

public delegate Func<Task>? Projector<T>(
    IAsyncDocumentSession session,
    object @event);