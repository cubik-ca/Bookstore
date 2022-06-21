using Bookstore.EventSourcing;
using Bookstore.SharedKernel;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Bookstore.WebApi;

public abstract class CommandApi<T> : ControllerBase
    where T : AggregateRoot<T>
{
    private readonly IBus _bus;
    private readonly ILogger _log;

    protected CommandApi(IBus bus, Token token, IHttpContextAccessor httpContext)
    {
        _log = Log.ForContext<CommandApi<T>>();
        _bus = bus;
        var principal = httpContext.HttpContext?.User;
        if (principal != null)
            token.Value = principal.Claims.SingleOrDefault(c => c.Type == "access_token")?.Value;
    }

    protected async Task<IActionResult> HandleCommand<TCommand>(
        TCommand command,
        Action<TCommand>? commandModifier = null)
        where TCommand : class
    {
        _log.Debug($"Handling HTTP request of type {typeof(TCommand).Name}");
        commandModifier?.Invoke(command);
        await _bus.Request<TCommand, CommandResponse>(command);
        return Ok();
    }
}