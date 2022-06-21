using Bookstore.EventSourcing;

namespace Bookstore.Domain.Subject.Country;

public class CountryId : AggregateId<Country>
{
    private CountryId(string? value) : base(value)
    {
    }

    public static CountryId FromInt(int? value)
    {
        return new(value?.ToString());
    }

    public static implicit operator int?(CountryId? value)
    {
        return value == null ? null : int.Parse(value.Value);
    }

    public static CountryId FromString(string? evId)
    {
        return new(evId);
    }
}