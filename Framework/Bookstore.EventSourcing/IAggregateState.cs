namespace Bookstore.EventSourcing;

public interface IAggregateState<T>
{
    string StreamName { get; }
    long Version { get; }
    T When(T state, object @event);
    string GetStreamName(Guid id);
}