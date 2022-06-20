namespace Bookstore.EventSourcing;

public abstract class AggregateId<T> : Value<AggregateId<T>>
{
    protected AggregateId(string? value)
    {
        if (value is null or null) throw new ArgumentNullException(nameof(value), "The Id cannot be empty");
        Value = value;
    }

    protected string Value { get; }

    public static implicit operator string(AggregateId<T> self)
    {
        return self.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}