using Bookstore.EventSourcing;
using MongoDB.Driver;

namespace Bookstore.MongoDB;

public class Projection : ISubscription
{
    private readonly IMongoClient _mongo;
    private readonly Projector _projector;

    public Projection(IMongoClient mongo, Projector projector)
    {
        _mongo = mongo;
        _projector = projector;
    }

    public async Task Project(object @event)
    {
        var handler = _projector.Invoke(_mongo, @event);
        if (handler == null) return;
        await handler();
    }
}

public delegate Func<Task>? Projector(IMongoClient mongo, object @event);