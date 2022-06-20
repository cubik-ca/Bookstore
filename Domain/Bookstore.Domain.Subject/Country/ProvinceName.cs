using Bookstore.EventSourcing;

namespace Bookstore.Domain.Subject.Country;

public class ProvinceName : Value<ProvinceName>
{
    private string Value { get; }

    private ProvinceName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));
        Value = value;
    }

    public static ProvinceName FromString(string? value) => new(value);

    public static implicit operator string?(ProvinceName? value) => value?.Value;
}