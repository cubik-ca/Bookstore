using Bookstore.EventSourcing;

namespace Bookstore.Domain.Subject.Country;

public class ProvinceAbbreviation : Value<ProvinceAbbreviation>
{
    private ProvinceAbbreviation(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));
        Value = value;
    }

    private string Value { get; }

    public static ProvinceAbbreviation FromString(string? value)
    {
        return new(value);
    }

    public static implicit operator string?(ProvinceAbbreviation? value)
    {
        return value?.Value;
    }
}