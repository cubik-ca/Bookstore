using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bookstore.SharedKernel;
using Bookstore.WebApi;
using Bookstore.WebApi.Filters;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var token = new Token();
var claimsHolder = new ClaimsHolder();
// Add services to the container.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
    {
        c.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
        c.TokenValidationParameters = new()
        {
            ValidAudience = builder.Configuration["Auth0:Audience"],
            ValidIssuer = $"https://{builder.Configuration["Auth0:Domain"]}"
        };
        c.Events = new()
        {
            OnTokenValidated = async context =>
            {
                if (context.SecurityToken is not JwtSecurityToken accessToken) return;
                token.Value = accessToken.RawData;
                if (context.Principal?.Identity is ClaimsIdentity identity)
                {
                    var userid = identity.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
                    var claims = claimsHolder[userid ?? throw new Exception("null user!")];
                    if (!claims.Any())
                    {
                        var httpClient = new HttpClient();
                        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken.RawData}");
                        claims = (await
                                httpClient.GetFromJsonAsync<Dictionary<string, object>>(
                                    $"https://{builder.Configuration["Auth0:Domain"]}/userinfo")
                            )?.Select(x =>
                                new Claim(x.Key, x.Value?.ToString() ?? throw new Exception("null claim??"))).ToList();
                        if (claims != null)
                            foreach (var claim in claims.ToList())
                                claimsHolder.AddClaim(userid, claim.Type, claim.Value);
                    }

                    identity.AddClaims(claims ?? Enumerable.Empty<Claim>().ToList());
                    identity.AddClaim(new Claim("access_token", accessToken.RawData));
                }
            }
        };
    });
builder.Services.AddSingleton(new MongoClient(builder.Configuration["MongoDB:Url"]));
builder.Services.AddMassTransit(mt =>
{
    mt.UsingRabbitMq((ctx, rmq) =>
    {
        rmq.UsePublishFilter(typeof(TokenPublishFilter<>), ctx);
        rmq.UseSendFilter(typeof(TokenSendFilter<>), ctx);
        rmq.Host(builder.Configuration["RabbitMQ:Url"]);
    });
});
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(x => x.FullName?.Replace("+", ".")); 
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Bookstore",
            Version = "v1"
        });
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new()
            {
                AuthorizationUrl = new($"https://{builder.Configuration["Auth0:Domain"]}/authorize"),
                TokenUrl = new($"https://{builder.Configuration["Auth0:Domain"]}/token"),
                Scopes = new Dictionary<string, string>()
                {
                    {"openid", "Authenticate with OIDC"},
                    {"profile", "Read your profile"},
                    {"offline_access", "Send your refresh token"},
                    {"email", "Your email address"}
                }
            }
        }
    });
    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                },
                Scheme = "oauth2",
                Name = "oauth2",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton(token);
builder.Services.AddSingleton(claimsHolder);

builder.Services.AddMvcCore(options => options.Conventions.Add(new CommandConvention()));
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.OAuthConfigObject.ClientId = builder.Configuration["Auth0:ClientId"];
        opt.OAuthConfigObject.AdditionalQueryStringParams = new() { { "audience", builder.Configuration["Auth0:Audience"] } };
    });
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllers();
    endpoints.MapFallbackToFile("index.html");
});


app.Run();