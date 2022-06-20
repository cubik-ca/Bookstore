using Bookstore.EventSourcing;
using MassTransit;
using Paragon.Shared.Messages;

namespace Bookstore.Services;

public class ConsumerBase<TAggregate, TCommand> : IConsumer<TCommand>
    where TAggregate : AggregateRoot<TAggregate>
    where TCommand : class
{
    protected readonly ApplicationService<TAggregate> Service;

    public ConsumerBase(ApplicationService<TAggregate> service)
    {
        Service = service;
    }

    public virtual async Task Consume(ConsumeContext<TCommand> context)
    {
        try
        {
            await Service.Handle(context.Message);
            await context.RespondAsync(new CommandResponse { Success = true });
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new CommandResponse
            { Success = false, Error = ex.Message, StackTrace = ex.StackTrace });
        }
    }
}