using Bookstore.EventSourcing;
using Bookstore.Message.Subject.Country;
using Bookstore.Services;

namespace Bookstore.Service.Subject.Country.CommandHandlers;

public class RemoveHandler : ConsumerBase<Domain.Subject.Country.Country, Commands.Remove>
{
    public RemoveHandler(ApplicationService<Domain.Subject.Country.Country> service) : base(service)
    {
    }
}