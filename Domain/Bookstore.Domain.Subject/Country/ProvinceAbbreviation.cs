using Bookstore.EventSourcing;

namespace Bookstore.Domain.Subject.Country;

public class ProvinceAbbreviation : Value<ProvinceAbbreviation>
{
    private string Value { get; }

    private ProvinceAbbreviation(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));
        Value = value;
    }

    public static ProvinceAbbreviation FromString(string? value) => new(value);

    public static implicit operator string?(ProvinceAbbreviation? value) => value?.Value;
}