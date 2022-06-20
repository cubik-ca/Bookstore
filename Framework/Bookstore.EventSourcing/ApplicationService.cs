using Serilog;

namespace Bookstore.EventSourcing;

public abstract class ApplicationService<T> where T : AggregateRoot<T>
{
    private static readonly ILogger Log = Serilog.Log.ForContext<ApplicationService<T>>();
    private readonly Dictionary<Type, Func<object, Task>> _handlers = new();
    protected readonly IAggregateStore Store;

    protected ApplicationService(IAggregateStore store)
    {
        Store = store;
    }

    public Task Handle<TCommand>(TCommand command) where TCommand : class
    {
        if (!_handlers.TryGetValue(typeof(TCommand), out var handler))
            throw new InvalidOperationException($"No registered handler for command {typeof(TCommand).Name}");
        Log.Debug($"Handling command {command}");
        return handler(command);
    }

    private void When<TCommand>(Func<TCommand, Task> handler) where TCommand : class
    {
        _handlers.Add(typeof(TCommand), c => handler((TCommand)c));
    }

    protected void CreateWhen<TCommand>(
        Func<TCommand, AggregateId<T>> getAggregateId,
        Func<TCommand, AggregateId<T>, T> creator
    ) where TCommand : class
    {
        When<TCommand>(
            async command =>
            {
                var aggregateId = getAggregateId(command);
                if (await Store.Exists(aggregateId))
                    throw new InvalidOperationException($"Entity with id {aggregateId} already exists");
                var aggregate = creator(command, aggregateId);
                await Store.Save(aggregate);
            }
        );
    }

    protected void UpdateWhen<TCommand>(
        Func<TCommand, AggregateId<T>> getAggregateId,
        Action<T, TCommand> updater
    ) where TCommand : class
    {
        When<TCommand>(
            async command =>
            {
                var aggregateId = getAggregateId(command);
                var aggregate = await Store.Load(aggregateId);
                if (aggregate == null)
                    throw new InvalidOperationException($"Entity with id {aggregateId} cannot be found");
                updater(aggregate, command);
                await Store.Save(aggregate);
            }
        );
    }
}