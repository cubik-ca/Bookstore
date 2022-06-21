using Bookstore.EventSourcing;

namespace Bookstore.Domain.Subject.Country;

public class Name : Value<Name>
{
    private Name(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));
        Value = value;
    }

    private string Value { get; }

    public static Name FromString(string? value)
    {
        return new(value);
    }

    public static implicit operator string?(Name? name)
    {
        return name?.Value;
    }
}