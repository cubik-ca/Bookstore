using static Bookstore.EventSourcing.TypeMapper;
using static Bookstore.Message.Subject.Country.Events;

namespace Bookstore.Service.Subject.Country;

public static class CountryEventMappings
{
    public static void MapEvents()
    {
        Map<Created>("Country.Created");
        Map<AbbreviationChanged>("Country.AbbreviationChanged");
        Map<NameChanged>("Country.NameChanged");
        Map<ProvinceAdded>("Country.ProvinceAdded"); 
        Map<ProvinceAbbreviationChanged>("Country.ProvinceAbbreviationChanged");
        Map<ProvinceNameChanged>("Country.ProvinceNameChanged");
        Map<ProvinceRemoved>("Country.ProvinceRemoved");
        Map<Removed>("Country.Removed");
    }
}