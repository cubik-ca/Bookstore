namespace Bookstore.Message.Subject.Country;

public static class Events
{
    public class Created
    {
        public string? Id { get; set; }
        public string? Abbreviation { get; set; }
        public string? Name { get; set; }
    }

    public class AbbreviationChanged
    {
        public string? Id { get; set; }
        public string? Abbreviation { get; set; }
    }

    public class NameChanged
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class ProvinceAdded
    {
        public string? Id { get; set; }
        public int? ProvinceId { get; set; }
        public string? Abbreviation { get; set; }
        public string? Name { get; set; }
    }

    public class ProvinceRemoved
    {
        public string? Id { get; set; }
        public int? ProvinceId { get; set; }
    }

    public class ProvinceAbbreviationChanged
    {
        public string? Id { get; set; }
        public int? ProvinceId { get; set; }
        public string? Abbreviation { get; set; }
    }

    public class ProvinceNameChanged
    {
        public string? Id { get; set; }
        public int? ProvinceId { get; set; }
        public string? Name { get; set; }
    }

    public class Removed
    {
        public string? Id { get; set; }
    }
}