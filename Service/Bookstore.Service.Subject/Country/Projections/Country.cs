using Bookstore.Message.Subject.Country;
using Bookstore.MongoDB;
using Bookstore.Service.Subject.Shared.Country;
using MongoDB.Driver;

namespace Bookstore.Service.Subject.Country.Projections;

public static class Country
{
    public static Func<Task>? GetHandler(IMongoClient mongo, object @event)
    {
        var countries = mongo.GetDatabase("Bookstore").GetCollection<ReadModels.Country>("Countries");
        return @event switch
        {
            Events.Created created => () => HandleCreated(countries, created),
            Events.AbbreviationChanged abbreviationChanged => () => HandleAbbreviationChanged(countries, abbreviationChanged),
            Events.NameChanged nameChanged => () => HandleNameChanged(countries, nameChanged),
            Events.Removed removed => () => HandleRemoved(countries, removed),
            _ => () => Task.CompletedTask
        };
    }

    private static async Task HandleCreated(
        IMongoCollection<ReadModels.Country> countries, 
        Events.Created @event)
    {
        await countries.UpsertItem(@event.Id ?? throw new Exception("Country Id is required"),
            country =>
            {
                country.Abbreviation = @event.Abbreviation;
                country.Name = @event.Name;
                return Task.CompletedTask;
            },
            () => Task.FromResult(new ReadModels.Country
            {
                Id = @event.Id,
                Abbreviation = @event.Abbreviation,
                Name = @event.Name
            }));
    }

    private static async Task HandleAbbreviationChanged(
        IMongoCollection<ReadModels.Country> countries,
        Events.AbbreviationChanged @event)
    {
        await countries.Update(@event.Id ?? throw new Exception("Country Id is required"),
            country =>
            {
                country.Abbreviation = @event.Abbreviation;
                return Task.CompletedTask;
            });
    }
    
    private static async Task HandleNameChanged(
        IMongoCollection<ReadModels.Country> countries,
        Events.NameChanged @event)
    {
        await countries.Update(@event.Id ?? throw new Exception("Country Id is required"),
            country =>
            {
                country.Name = @event.Name;
                return Task.CompletedTask;
            }); 
    }

    private static async Task HandleRemoved(
        IMongoCollection<ReadModels.Country> countries,
        Events.Removed @event)
    {
        await countries.Delete(@event.Id ?? throw new Exception("Country Id is required"));
    }
}