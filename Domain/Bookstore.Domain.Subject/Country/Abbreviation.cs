using Bookstore.EventSourcing;

namespace Bookstore.Domain.Subject.Country;

public class Abbreviation : Value<Abbreviation>
{
    private string Value { get; }

    private Abbreviation(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));
        Value = value;
    }

    public static Abbreviation FromString(string? value) => new(value);

    public static implicit operator string?(Abbreviation? value) => value?.Value;
}