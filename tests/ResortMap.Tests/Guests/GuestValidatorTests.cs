using System.Text.Json;
using ResortMap.Guests;
using Xunit;

namespace ResortMap.Tests.Guests;

public class GuestValidatorTests
{
    private GuestValidator CreateValidatorWithGuests(params Guest[] guests)
    {
        var validator = new GuestValidator();
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, JsonSerializer.Serialize(guests));
        validator.Load(tempFile);
        File.Delete(tempFile);
        return validator;
    }

    [Fact]
    public void ValidRoomAndName_ReturnsTrue()
    {
        var validator = CreateValidatorWithGuests(new Guest("101", "Alice Smith"));

        Assert.True(validator.IsValid("101", "Alice Smith"));
    }

    [Fact]
    public void WrongName_ReturnsFalse()
    {
        var validator = CreateValidatorWithGuests(new Guest("101", "Alice Smith"));

        Assert.False(validator.IsValid("101", "Bob Jones"));
    }

    [Fact]
    public void WrongRoom_ReturnsFalse()
    {
        var validator = CreateValidatorWithGuests(new Guest("101", "Alice Smith"));

        Assert.False(validator.IsValid("999", "Alice Smith"));
    }

    [Fact]
    public void UnknownGuest_ReturnsFalse()
    {
        var validator = CreateValidatorWithGuests(new Guest("101", "Alice Smith"));

        Assert.False(validator.IsValid("999", "Nobody"));
    }

    [Fact]
    public void CaseSensitive_NameMismatch_ReturnsFalse()
    {
        var validator = CreateValidatorWithGuests(new Guest("101", "Alice Smith"));

        Assert.False(validator.IsValid("101", "alice smith"));
    }

    [Fact]
    public void MultipleGuests_EachValidatesIndependently()
    {
        var validator = CreateValidatorWithGuests(
            new Guest("101", "Alice Smith"),
            new Guest("102", "Bob Jones")
        );

        Assert.True(validator.IsValid("101", "Alice Smith"));
        Assert.True(validator.IsValid("102", "Bob Jones"));
        Assert.False(validator.IsValid("101", "Bob Jones"));
        Assert.False(validator.IsValid("102", "Alice Smith"));
    }

    [Fact]
    public void EmptyGuestList_AlwaysReturnsFalse()
    {
        var validator = CreateValidatorWithGuests();

        Assert.False(validator.IsValid("101", "Alice Smith"));
    }
}
