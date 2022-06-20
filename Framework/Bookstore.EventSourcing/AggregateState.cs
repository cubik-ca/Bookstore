using Force.DeepCloner;

namespace Bookstore.EventSourcing;

public abstract class AggregateState<T> : IAggregateState<T> where T : class, new()
{
    public Guid Id { get; protected set; }
    public string StreamName => GetStreamName(Id);
    public long Version { get; protected set; }

    public string GetStreamName(Guid id)
    {
        return $"{typeof(T).Name}-{id:N}";
    }

    public abstract T When(T state, object @event);
    protected abstract bool EnsureValidState(T newState);

    private T Apply(T state, object @event)
    {
        var newState = state.DeepClone();
        newState = When(newState, @event);
        if (!EnsureValidState(newState))
            throw new InvalidEntityState(this, "Post-checks failed");
        return newState;
    }

    public Result NoEvents()
    {
        return new(this as T ?? throw new ArgumentException("Aggregate is of wrong type"),
            new List<object>());
    }

    public Result Apply(params object[] events)
    {
        var newState = this as T ?? throw new ArgumentException("Aggregate is of wrong type");
        newState = events.Aggregate(newState, Apply);
        return new Result(newState, events);
    }

    private class InvalidEntityState : Exception
    {
        public InvalidEntityState(object entity, string message)
            : base($"Entity {entity.GetType().Name} state change rejected, {message}")
        {
        }
    }

    public class Result
    {
        public Result(T state, IEnumerable<object> events)
        {
            State = state;
            Events = events;
        }

        public T State { get; }
        public IEnumerable<object> Events { get; }
    }
}