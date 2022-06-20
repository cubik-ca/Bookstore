namespace Bookstore.EventSourcing;

public interface IApplicationService
{
    Task Handle(object command);
}