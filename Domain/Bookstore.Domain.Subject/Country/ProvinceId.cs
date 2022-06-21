using Bookstore.EventSourcing;

namespace Bookstore.Domain.Subject.Country;

public class ProvinceId : Value<ProvinceId>
{
    private ProvinceId(int? value)
    {
        if (!value.HasValue)
            throw new ArgumentNullException(nameof(value));
        Value = value.Value;
    }

    private int Value { get; }

    public static ProvinceId FromInt(int? value)
    {
        return new(value);
    }

    public static implicit operator int?(ProvinceId? value)
    {
        return value?.Value;
    }
}