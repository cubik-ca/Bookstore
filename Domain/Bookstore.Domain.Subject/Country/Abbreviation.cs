using Bookstore.EventSourcing;

namespace Bookstore.Domain.Subject.Country;

public class Abbreviation : Value<Abbreviation>
{
    private Abbreviation(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));
        Value = value;
    }

    private string Value { get; }

    public static Abbreviation FromString(string? value)
    {
        return new(value);
    }

    public static implicit operator string?(Abbreviation? value)
    {
        return value?.Value;
    }
}