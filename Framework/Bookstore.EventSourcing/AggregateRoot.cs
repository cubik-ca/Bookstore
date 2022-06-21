namespace Bookstore.EventSourcing;

public abstract class AggregateRoot<T> : IInternalEventHandler where T : AggregateRoot<T>
{
    private readonly List<object> _changes = new();

    protected AggregateRoot(AggregateId<T> id)
    {
        Id = id;
    }

    public AggregateId<T> Id { get; protected set; }
    public int Version { get; protected set; } = -1;


    void IInternalEventHandler.Handle(object @event)
    {
        When(@event);
    }

    protected abstract void EnsureValidState();
    protected abstract void When(object? @event);

    protected void Apply(object @event)
    {
        When(@event);
        EnsureValidState();
        _changes.Add(@event);
    }

    public IEnumerable<object> GetChanges()
    {
        return _changes.AsEnumerable();
    }

    public void Load(IEnumerable<object?> history)
    {
        foreach (var ev in history)
        {
            When(ev);
            Version++;
        }
    }

    public void ClearChanges()
    {
        _changes.Clear();
    }

    protected static void ApplyToEntity(IInternalEventHandler? entity, object @event)
    {
        entity?.Handle(@event);
    }
}