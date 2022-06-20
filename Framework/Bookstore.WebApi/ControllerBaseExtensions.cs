using Microsoft.AspNetCore.Mvc;

namespace Bookstore.WebApi;

public static class ControllerBaseExtensions
{
    public static async Task<IActionResult> HandleCommand(this ControllerBase _, Task handler)
    {
        try
        {
            await handler;
            return new OkResult();
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(
                new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
        }
    }
}