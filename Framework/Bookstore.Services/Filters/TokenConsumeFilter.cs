using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Bookstore.EventStore;
using Bookstore.SharedKernel;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Bookstore.Services.Filters;

public class TokenConsumeFilter<T> : IFilter<ConsumeContext<T>>
    where T : class
{
    // this should be the same reference as the hosted service, therefore we can set its principal
    private readonly EventStoreService _eventStoreService;
    private readonly Token _token;
    private readonly IConfiguration _configuration;

    public TokenConsumeFilter(IConfiguration configuration, Token token, EventStoreService service)
    {
        _token = token;
        _eventStoreService = service;
        _configuration = configuration;
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var token = context.Headers.Get<string>("access_token");
        if (string.IsNullOrWhiteSpace(token))
        {
            await next.Send(context);
            return;
        }

        _token.Value = token;
        var domain = _configuration["Auth0:Domain"];
        var configuration = await OpenIdConnectConfigurationRetriever.GetAsync(
            $"https://{domain}/.well-known/openid-configuration", CancellationToken.None);
        TokenValidationParameters validationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://{domain}/",
            ValidateAudience = true,
            ValidAudience = _configuration["Auth0:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = configuration.SigningKeys,
            NameClaimType = "email"
        };
        var handler = new JwtSecurityTokenHandler();
        // will throw an exception if invalid. this is what we want, because it will prevent the pipeline from continuing to execute
        Thread.CurrentPrincipal = handler.ValidateToken(token, validationParameters, out var securityToken);
        var accessToken = securityToken as JwtSecurityToken;
        if (accessToken == null) throw new Exception("Invalid token");
        if (Thread.CurrentPrincipal is ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity is not ClaimsIdentity identity)
                throw new Exception("null identity??");
            using var httpClient = new HttpClient();
            // load the roles from the userinfo endpoint
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken?.RawData);
            var claims = await httpClient.GetFromJsonAsync<Dictionary<string, object>>($"https://{domain}/userinfo");
            if (claims != null)
                foreach (var (k, v) in claims)
                    identity.AddClaim(new Claim(k, v.ToString() ?? throw new Exception("null value")));
            _eventStoreService.Principal = (ClaimsPrincipal) Thread.CurrentPrincipal;
        }
        else
            throw new Exception("no claims principal??");
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
    }
}