using System;
using System.Linq;
using Bookstore.Domain.Subject.Country;
using Xunit;

namespace Bookstore.Test.Domain.Subject;

public class CountryTests
{
    private static int _provinceId;

    private static readonly Country Canada = new(
        CountryId.FromString(Ulid.NewUlid().ToString()),
        Abbreviation.FromString("CA"),
        Name.FromString("Canada"));

    [Fact]
    public void CanCreateCountry()
    {
        var countryId = Ulid.NewUlid().ToString();
        const string countryName = "Canada";
        const string countryAbbreviation = "CA";
        var country = new Country(
            CountryId.FromString(countryId),
            Abbreviation.FromString(countryAbbreviation),
            Name.FromString(countryName));
        Assert.Equal(countryName, country.Name);
        Assert.Equal(countryAbbreviation, country.Abbreviation);
        Assert.Equal(countryId, country.Id);
        Assert.NotNull(country.Provinces);
        Assert.Equal(0, country.Provinces.Count);
    }

    [Fact]
    public void CanSetAbbreviation()
    {
        const string newAbbreviation = "US";
        Canada.SetAbbreviation(Abbreviation.FromString(newAbbreviation));
        Assert.Equal(Canada.Abbreviation, newAbbreviation);
        // cleanup
        Canada.SetAbbreviation(Abbreviation.FromString("CA"));
    }

    [Fact]
    public void CanSetName()
    {
        const string newName = "United States";
        Canada.SetName(Name.FromString(newName));
        Assert.Equal(Canada.Name, newName);
        // cleanup
        Canada.SetName(Name.FromString("Canada"));
    }

    [Fact]
    public void CanCreateProvince()
    {
        var provinceId = ++_provinceId;
        const string provinceName = "Ontario";
        const string provinceAbbreviation = "ON";
        var country = Canada;
        country.AddProvince(
            ProvinceId.FromInt(provinceId),
            ProvinceAbbreviation.FromString(provinceAbbreviation),
            ProvinceName.FromString(provinceName));
        Assert.Equal(1, country.Provinces.Count);
        var province = country.Provinces[0];
        Assert.True(_provinceId == province.Id);
        Assert.Equal(provinceAbbreviation, province.Abbreviation);
        Assert.Equal(provinceName, province.Name);
        // cleanup
        country.RemoveProvince(ProvinceId.FromInt(provinceId));
    }

    [Fact]
    public void CanRemoveProvince()
    {
        var country = Canada;
        var count = Canada.Provinces.Count;
        var provinceId = ++_provinceId;
        const string provinceName = "Manitoba";
        const string provinceAbbreviation = "MB";
        country.AddProvince(
            ProvinceId.FromInt(provinceId),
            ProvinceAbbreviation.FromString(provinceAbbreviation),
            ProvinceName.FromString(provinceName));
        country.RemoveProvince(ProvinceId.FromInt(provinceId));
        Assert.True(country.Provinces.All(p => p.Id != provinceId));
        Assert.Equal(count, country.Provinces.Count);
        // no cleanup required
    }

    [Fact]
    public void CanSetProvinceAbbreviation()
    {
        var provinceId = ++_provinceId;
        const string provinceName = "Alberta";
        const string provinceAbbreviation = "AB";
        Canada.AddProvince(ProvinceId.FromInt(provinceId), ProvinceAbbreviation.FromString(provinceAbbreviation),
            ProvinceName.FromString(provinceName));
        Canada.SetProvinceAbbreviation(ProvinceId.FromInt(provinceId), ProvinceAbbreviation.FromString("SK"));
        var province = Canada.Provinces.First(p => p.Id == provinceId);
        Assert.Equal("SK", province.Abbreviation);
        // cleanup
        Canada.RemoveProvince(ProvinceId.FromInt(provinceId));
    }

    [Fact]
    public void CanSetProvinceName()
    {
        var provinceId = ++_provinceId;
        const string provinceName = "Alberta";
        const string provinceAbbreviation = "AB";
        Canada.AddProvince(ProvinceId.FromInt(provinceId), ProvinceAbbreviation.FromString(provinceAbbreviation),
            ProvinceName.FromString(provinceName));
        Canada.SetProvinceName(ProvinceId.FromInt(provinceId), ProvinceName.FromString("Saskatchewan"));
        var province = Canada.Provinces.First(p => p.Id == provinceId);
        Assert.Equal("Saskatchewan", province.Name);
        // cleanup
        Canada.RemoveProvince(ProvinceId.FromInt(provinceId));
    }
}