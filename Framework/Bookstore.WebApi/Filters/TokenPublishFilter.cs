using Bookstore.SharedKernel;
using MassTransit;

namespace Bookstore.WebApi.Filters;

public class TokenPublishFilter<T> : IFilter<PublishContext<T>>
    where T : class
{
    private readonly Token _token;

    public TokenPublishFilter(Token token)
    {
        _token = token;
    }

    public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {
        if (!string.IsNullOrWhiteSpace(_token.Value))
            context.Headers.Set("access_token", _token.Value);
        return next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
    }
}