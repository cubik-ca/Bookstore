namespace Bookstore.Message.Subject.Country;

public static class Commands
{
    public class Create
    {
        public string? Id { get; set; }
        public string? Abbreviation { get; set; }
        public string? Name { get; set; }
    }

    public class SetAbbreviation
    {
        public string? Id { get; set; }
        public string? Abbreviation { get; set; }
    }

    public class SetName
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class AddProvince
    {
        public string? Id { get; set; }
        public int? ProvinceId { get; set; }
        public string? Abbreviation { get; set; }
        public string? Name { get; set; }
    }

    public class RemoveProvince
    {
        public string? Id { get; set; }
        public int? ProvinceId { get; set; }
    }

    public class SetProvinceAbbreviation
    {
        public string? Id { get; set; }
        public int? ProvinceId { get; set; }
        public string? Abbreviation { get; set; }
    }

    public class SetProvinceName
    {
        public string? Id { get; set; }
        public int? ProvinceId { get; set; }
        public string? Name { get; set; }
    }

    public class Remove
    {
        public string? Id { get; set; }
    }
}