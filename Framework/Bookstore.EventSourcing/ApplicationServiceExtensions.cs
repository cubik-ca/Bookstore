namespace Bookstore.EventSourcing;

public static class ApplicationServiceExtensions
{
    public static async Task HandleUpdate<T>(
        this IApplicationService applicationService,
        IAggregateStore store,
        AggregateId<T> aggregateId,
        Action<T> operation
    ) where T : AggregateRoot<T>
    {
        var aggregate = await store.Load(aggregateId);
        if (aggregate == null)
            throw new InvalidOperationException($"Entity with id {aggregateId} cannot be found");
        operation(aggregate);
        await store.Save(aggregate);
    }
}