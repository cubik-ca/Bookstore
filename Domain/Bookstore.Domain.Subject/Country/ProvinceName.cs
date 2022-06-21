using Bookstore.EventSourcing;

namespace Bookstore.Domain.Subject.Country;

public class ProvinceName : Value<ProvinceName>
{
    private ProvinceName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));
        Value = value;
    }

    private string Value { get; }

    public static ProvinceName FromString(string? value)
    {
        return new(value);
    }

    public static implicit operator string?(ProvinceName? value)
    {
        return value?.Value;
    }
}