namespace Bookstore.EventSourcing;

public class CommandService<T> where T : class, IAggregateState<T>, new()
{
    protected CommandService(IFunctionalAggregateStore store)
    {
        Store = store;
    }

    private IFunctionalAggregateStore Store { get; }

    protected async Task Handle(
        Guid id,
        Func<T, AggregateState<T>.Result> update)
    {
        var state = await Store.Load<T>(id);
        await Store.Save(state.Version, update(state));
    }
}