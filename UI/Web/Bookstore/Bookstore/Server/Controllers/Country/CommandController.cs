using Bookstore.Message.Subject.Country;
using Bookstore.SharedKernel;
using Bookstore.WebApi;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Server.Controllers.Country;

[Authorize, Route("api/country"), ApiController]
public class CommandController : CommandApi<Domain.Subject.Country.Country>
{
    public CommandController(IBus bus, Token token, IHttpContextAccessor httpContext) : base(bus, token, httpContext)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Create(Commands.Create command)
        => await HandleCommand(command);
    
    [HttpDelete]
    public async Task<IActionResult> Remove([FromQuery] Commands.Remove command)
        => await HandleCommand(command);
}