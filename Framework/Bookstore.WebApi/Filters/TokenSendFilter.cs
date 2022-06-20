using Bookstore.SharedKernel;
using MassTransit;

namespace Bookstore.WebApi.Filters;

public class TokenSendFilter<T> : IFilter<SendContext<T>>
    where T : class
{
    private readonly Token _token;

    public TokenSendFilter(Token token)
    {
        _token = token;
    }

    public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        if (!string.IsNullOrWhiteSpace(_token.Value))
            context.Headers.Set("access_token", _token.Value);
        return next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
    }
}