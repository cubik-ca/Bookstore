namespace Bookstore.EventSourcing;

public interface ISubscription
{
    Task Project(object @event);
}