using Bookstore.EventSourcing;
using Serilog;

namespace Bookstore.EventStore;

public abstract class ReactorBase : ISubscription
{
    private readonly ILogger _log;
    private readonly Reactor? _reactor;

    public ReactorBase(Reactor reactor)
    {
        _reactor = reactor;
        _log = Log.ForContext(GetType());
    }

    public Task Project(object @event)
    {
        var handler = _reactor?.Invoke(@event);
        if (handler == null) return Task.CompletedTask;
        _log.Debug($"Reacting to event {@event}");
        return handler();
    }
}

public delegate Func<Task> Reactor(object @event);