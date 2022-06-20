namespace Bookstore.EventSourcing;

public interface IFunctionalAggregateStore
{
    Task Save<T>(long version, AggregateState<T>.Result update) where T : class, IAggregateState<T>, new();
    Task<T> Load<T>(Guid id) where T : IAggregateState<T>, new();
}