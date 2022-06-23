using Bookstore.EventSourcing;
using Bookstore.Message.Subject.Country;
using Bookstore.Services;

namespace Bookstore.Service.Subject.Country.CommandHandlers;

public class CreateHandler : ConsumerBase<Domain.Subject.Country.Country, Commands.Create>
{
    public CreateHandler(ApplicationService<Domain.Subject.Country.Country> service) : base(service)
    {
    }
}