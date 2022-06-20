using Bookstore.EventSourcing;

namespace Bookstore.Domain.Subject.Country;

public class ProvinceId : Value<ProvinceId>
{
    private int Value { get; }

    private ProvinceId(int? value)
    {
        if (!value.HasValue)
            throw new ArgumentNullException(nameof(value));
        Value = value.Value;
    }

    public static ProvinceId FromInt(int? value) => new(value);

    public static implicit operator int?(ProvinceId? value) => value?.Value;
}