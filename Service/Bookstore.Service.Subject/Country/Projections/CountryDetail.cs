using Bookstore.Message.Subject.Country;
using Bookstore.MongoDB;
using Bookstore.Service.Subject.Shared.Country;
using MongoDB.Driver;

namespace Bookstore.Service.Subject.Country.Projections;

public static class CountryDetail
{
    public static Func<Task>? GetHandler(IMongoClient mongo, object @event)
    {
        var countries = mongo.GetDatabase("Bookstore").GetCollection<ReadModels.CountryDetail>("CountryDetails");
        return @event switch
        {
            Events.Created created => () => HandleCreated(countries, created),
            Events.AbbreviationChanged abbreviationChanged => () => HandleAbbreviationChanged(countries, abbreviationChanged),
            Events.NameChanged nameChanged => () => HandleNameChanged(countries, nameChanged),
            Events.ProvinceAdded provinceAdded => () => HandleProvinceAdded(countries, provinceAdded),
            Events.ProvinceRemoved provinceRemoved => () => HandleProvinceRemoved(countries, provinceRemoved),
            Events.ProvinceAbbreviationChanged provinceAbbreviationChanged => () => HandleProvinceAbbreviationChanged(countries, provinceAbbreviationChanged),
            Events.ProvinceNameChanged provinceNameChanged => () => HandleProvinceNameChanged(countries, provinceNameChanged),
            Events.Removed removed => () => HandleRemoved(countries, removed),
            _ => () => Task.CompletedTask
        };
    }

    private static async Task HandleCreated(
        IMongoCollection<ReadModels.CountryDetail> countries, 
        Events.Created @event)
    {
        await countries.UpsertItem(@event.Id ?? throw new Exception("Country Id is required"),
            country =>
            {
                country.Abbreviation = @event.Abbreviation;
                country.Name = @event.Name;
                country.Provinces = new List<ReadModels.Province>();
                return Task.CompletedTask;
            },
            () => Task.FromResult(new ReadModels.CountryDetail
            {
                Id = @event.Id,
                Abbreviation = @event.Abbreviation,
                Name = @event.Name,
                Provinces = new List<ReadModels.Province>()
            }));
    }

    private static async Task HandleAbbreviationChanged(
        IMongoCollection<ReadModels.CountryDetail> countries,
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
        IMongoCollection<ReadModels.CountryDetail> countries,
        Events.NameChanged @event)
    {
        await countries.Update(@event.Id ?? throw new Exception("Country Id is required"),
            country =>
            {
                country.Name = @event.Name;
                return Task.CompletedTask;
            }); 
    }

    private static async Task HandleProvinceAdded(
        IMongoCollection<ReadModels.CountryDetail> countries,
        Events.ProvinceAdded @event)
    {
        await countries.Update(@event.Id ?? throw new Exception("Country Id is required"),
            country =>
            {
                country.Provinces ??= new List<ReadModels.Province>();
                country.Provinces.Add(new ReadModels.Province
                {
                    Id = @event.ProvinceId,
                    Abbreviation = @event.Abbreviation,
                    Name = @event.Name
                });
                return Task.CompletedTask;
            }); 
    }

    private static async Task HandleProvinceAbbreviationChanged(
        IMongoCollection<ReadModels.CountryDetail> countries,
        Events.ProvinceAbbreviationChanged @event)
    {
        await countries.Update(@event.Id ?? throw new Exception("Country Id is required"),
            country =>
            {
                country.Provinces ??= new List<ReadModels.Province>();
                var province = country.Provinces.FirstOrDefault(p => p.Id == @event.ProvinceId);
                if (province == null)
                    throw new Exception("Province not found");
                province.Abbreviation = @event.Abbreviation;
                return Task.CompletedTask;
            });
    }
    
    private static async Task HandleProvinceNameChanged(
        IMongoCollection<ReadModels.CountryDetail> countries,
        Events.ProvinceNameChanged @event)
    {
        await countries.Update(@event.Id ?? throw new Exception("Country Id is required"),
            country =>
            {
                country.Provinces ??= new List<ReadModels.Province>();
                var province = country.Provinces.FirstOrDefault(p => p.Id == @event.ProvinceId);
                if (province == null)
                    throw new Exception("Province not found");
                province.Name = @event.Name;
                return Task.CompletedTask;
            }); 
    }

    private static async Task HandleProvinceRemoved(
        IMongoCollection<ReadModels.CountryDetail> countries,
        Events.ProvinceRemoved @event)
    {
          await countries.Update(@event.Id ?? throw new Exception("Country Id is required"),
              country =>
              {
                  country.Provinces = country.Provinces?.Where(p => p.Id != @event.ProvinceId).ToList();
                  return Task.CompletedTask;
              });      
    }

    private static async Task HandleRemoved(
        IMongoCollection<ReadModels.CountryDetail> countries,
        Events.Removed @event)
    {
        await countries.Delete(@event.Id ?? throw new Exception("Country Id is required"));
    }
}