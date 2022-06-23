namespace Bookstore.Service.Subject.Shared.Country;

public static class ReadModels
{
    public class Country
    {
        public string? Id { get; set; }
        public string? Abbreviation { get; set; }
        public string? Name { get; set; }
    }

    public class Province
    {
        public int? Id { get; set; }
        public string? Abbreviation { get; set; }
        public string? Name { get; set; }
    }

    public class CountryDetail : Country
    {
        public IList<Province>? Provinces { get; set; }
    }
}