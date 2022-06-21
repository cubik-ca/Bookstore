using Bookstore.EventSourcing;
using Bookstore.Message.Subject.Country;

namespace Bookstore.Domain.Subject.Country;

public class Province : Entity<ProvinceId>
{
    public Province(ProvinceId id, ProvinceAbbreviation abbreviation, ProvinceName name, Action<object> applier) :
        base(applier)
    {
        Id = id;
        Abbreviation = abbreviation;
        Name = name;
    }

    public ProvinceAbbreviation? Abbreviation { get; private set; }
    public ProvinceName? Name { get; private set; }

    protected override void When(object @event)
    {
        switch (@event)
        {
            case Events.ProvinceAbbreviationChanged ev:
                Abbreviation = ProvinceAbbreviation.FromString(ev.Abbreviation);
                break;
            case Events.ProvinceNameChanged ev:
                Name = ProvinceName.FromString(ev.Name);
                break;
        }
    }
}