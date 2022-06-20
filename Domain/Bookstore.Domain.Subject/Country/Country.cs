using Bookstore.EventSourcing;
using Bookstore.Message.Subject.Country;

namespace Bookstore.Domain.Subject.Country;

public class Country : AggregateRoot<Country>
{
    public Abbreviation Abbreviation { get; private set; }
    public Name Name { get; private set; }
    public IList<Province> Provinces { get; private set; }

    public Country(
        CountryId id,
        Abbreviation abbreviation,
        Name name) : base(id)

    {
        Abbreviation = abbreviation;
        Name = name;
        Provinces = new List<Province>();
        Apply(new Events.Created
        {
            Id = id,
            Abbreviation = abbreviation,
            Name = name
        });
    }

    public void SetAbbreviation(Abbreviation abbreviation)
        => Apply(new Events.AbbreviationChanged
        {
            Id = Id,
            Abbreviation = abbreviation
        });

    public void SetName(Name name)
        => Apply(new Events.NameChanged
        {
            Id = Id,
            Name = name
        });

    public void AddProvince(ProvinceId provinceId, ProvinceAbbreviation abbreviation, ProvinceName name)
        => Apply(new Events.ProvinceAdded
        {
            Id = Id,
            ProvinceId = provinceId,
            Abbreviation = abbreviation,
            Name = name
        });

    public void RemoveProvince(ProvinceId provinceId)
        => Apply(new Events.ProvinceRemoved
        {
            Id = Id,
            ProvinceId = provinceId
        });

    public void SetProvinceAbbreviation(ProvinceId provinceId, ProvinceAbbreviation abbreviation)
        => Apply(new Events.ProvinceAbbreviationChanged
        {
            Id = Id,
            ProvinceId = provinceId,
            Abbreviation = abbreviation
        });

    public void SetProvinceName(ProvinceId provinceId, ProvinceName name)
        => Apply(new Events.ProvinceNameChanged
        {
            Id = Id,
            ProvinceId = provinceId,
            Name = name
        });

    protected override void EnsureValidState()
    {
    }

    protected override void When(object? @event)
    {
        Province? province;
        switch (@event)
        {
            case Events.Created ev:
                Id = CountryId.FromString(ev.Id);
                Abbreviation = Abbreviation.FromString(ev.Abbreviation);
                Name = Name.FromString(ev.Name);
                Provinces = new List<Province>();
                break;
            case Events.AbbreviationChanged ev:
                Abbreviation = Abbreviation.FromString(ev.Abbreviation);
                break;
            case Events.NameChanged ev:
                Name = Name.FromString(ev.Name);
                break;
            case Events.ProvinceAdded ev:
                Provinces.Add(new(
                    ProvinceId.FromInt(ev.ProvinceId),
                    ProvinceAbbreviation.FromString(ev.Abbreviation),
                    ProvinceName.FromString(ev.Name),
                    Apply));
                break;
            case Events.ProvinceAbbreviationChanged ev:
                province = Provinces.FirstOrDefault(p => p.Id == ev.ProvinceId);
                ApplyToEntity(province, ev);
                break;
            case Events.ProvinceNameChanged ev:
                province = Provinces.FirstOrDefault(p => p.Id == ev.ProvinceId);
                ApplyToEntity(province, ev);
                break;
            case Events.ProvinceRemoved ev:
                Provinces = Provinces.Where(p => p.Id != ev.ProvinceId).ToList();
                break;
        }
    }
}