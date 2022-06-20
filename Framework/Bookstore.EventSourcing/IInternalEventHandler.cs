namespace Bookstore.EventSourcing;

public interface IInternalEventHandler
{
    void Handle(object @event);
}