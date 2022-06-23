using System.Reflection;
using Bookstore.EventSourcing;
using Bookstore.EventStore;
using Bookstore.MongoDB;
using Bookstore.Service.Subject.Country;
using Bookstore.Service.Subject.Country.Projections;
using Bookstore.Services.Filters;
using Bookstore.SharedKernel;
using EventStore.Client;
using MassTransit;
using MongoDB.Driver;
using Country = Bookstore.Domain.Subject.Country.Country;

CountryEventMappings.MapEvents();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var esSettings = EventStoreClientSettings.Create(context.Configuration["EventStore:Url"]);
        var esClient = new EventStoreClient(esSettings);
        var mongoClient = new MongoClient(context.Configuration["MongoDB:Url"]);
        services.AddSingleton(esClient);
        services.AddSingleton(mongoClient);
        services.AddSingleton<IAggregateStore, EsAggregateStore>();
        services.AddSingleton<IFunctionalAggregateStore, FunctionalStore>();
        services.AddMassTransit(mt =>
        {
            mt.AddConsumers(Assembly.GetExecutingAssembly());
            mt.UsingRabbitMq((ctx, rmq) =>
            {
                rmq.Host(new Uri(context.Configuration["RabbitMQ:Url"]));
                rmq.ConfigureEndpoints(ctx);
                rmq.UseConsumeFilter(typeof(TokenConsumeFilter<>), ctx);
            });
        });
        services.AddSingleton<ApplicationService<Country>, CountryService>();
        var sm = new SubscriptionManager(
            esClient,
            new CheckpointStore(mongoClient, "Subject"),
            new Projection(mongoClient, Bookstore.Service.Subject.Country.Projections.Country.GetHandler),
            new Projection(mongoClient, CountryDetail.GetHandler));
        services.AddSingleton(sm);
        services.AddSingleton<Token>();
        services.AddSingleton<EventStoreService>();
        services.AddHostedService(sp => sp.GetRequiredService<EventStoreService>());
    })
    .Build();

await host.RunAsync();