using Bookstore.Domain.Subject.Country;
using Bookstore.EventSourcing;
using Bookstore.Message.Subject.Country;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace Bookstore.Service.Subject.Country;

public class CountryService : ApplicationService<Domain.Subject.Country.Country>
{
    public CountryService(IAggregateStore store) : base(store)
    {
        CreateWhen<Commands.Create>(
            cmd => CountryId.FromString(cmd.Id),
            (cmd, id) => new Domain.Subject.Country.Country(
                CountryId.FromString(cmd.Id),
                Abbreviation.FromString(cmd.Abbreviation),
                Name.FromString(cmd.Name)));
        UpdateWhen<Commands.Remove>(
            cmd => CountryId.FromString(cmd.Id),
            (country, cmd) => country.Remove());
    }
}