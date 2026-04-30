using NSubstitute;
using ResortMap.Booking;
using ResortMap.Guests;
using ResortMap.Maps;
using Xunit;

namespace ResortMap.Tests.Booking;

public class BookingServiceTests
{
    private static Map CreateMapWithCabanas(params (int row, int col)[] positions)
    {
        if (positions.Length == 0)
            return Map.Parse(["."]);

        int rows = positions.Max(p => p.row) + 1;
        int cols = positions.Max(p => p.col) + 1;
        var lines = new string[rows];

        for (int r = 0; r < rows; r++)
        {
            var chars = new char[cols];
            Array.Fill(chars, '.');
            foreach (var pos in positions.Where(p => p.row == r))
                chars[pos.col] = 'W';
            lines[r] = new string(chars);
        }

        return Map.Parse(lines);
    }

    private static BookingService CreateService(Map map, IGuestValidator? validator = null)
    {
        validator ??= Substitute.For<IGuestValidator>();
        validator.IsValid(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        return new BookingService(validator, map);
    }

    [Fact]
    public void GetAllCabanas_ReturnsCabanasFromMapTiles()
    {
        var map = CreateMapWithCabanas((0, 1), (2, 3));
        var service = CreateService(map);

        var cabanas = service.GetAllCabanas().ToList();

        Assert.Equal(2, cabanas.Count);
        Assert.Contains(cabanas, c => c.Id == "W-0-1");
        Assert.Contains(cabanas, c => c.Id == "W-2-3");
    }

    [Fact]
    public void GetAllCabanas_ExcludesNonCabanaTiles()
    {
        // '#' = Path, 'W' = Cabana, 'p' = Pool
        var map = Map.Parse(["#Wp"]);
        var service = CreateService(map);

        var cabanas = service.GetAllCabanas().ToList();

        Assert.Single(cabanas);
        Assert.Equal("W-0-1", cabanas[0].Id);
    }

    [Fact]
    public void GetAllCabanas_AllStartAsAvailable()
    {
        var map = CreateMapWithCabanas((0, 0), (1, 1));
        var service = CreateService(map);

        Assert.All(service.GetAllCabanas(), c => Assert.Equal(CabanaStatus.Available, c.Status));
    }

    [Fact]
    public void BookCabana_ValidRequest_ReturnsSuccessWithMessage()
    {
        var map = CreateMapWithCabanas((0, 0));
        var service = CreateService(map);

        var result = service.BookCabana("W-0-0", new BookingRequest("101", "Alice Smith"));

        Assert.True(result.Success);
        Assert.Contains("Alice Smith", result.Message);
        Assert.Contains("101", result.Message);
    }

    [Fact]
    public void BookCabana_NonExistentId_ReturnsNotFoundError()
    {
        var map = CreateMapWithCabanas((0, 0));
        var service = CreateService(map);

        var result = service.BookCabana("W-99-99", new BookingRequest("101", "Alice"));

        Assert.False(result.Success);
        Assert.Equal(BookingError.NotFound, result.Error);
    }

    [Fact]
    public void BookCabana_AlreadyBooked_ReturnsAlreadyBookedError()
    {
        var map = CreateMapWithCabanas((0, 0));
        var service = CreateService(map);

        service.BookCabana("W-0-0", new BookingRequest("101", "Alice"));
        var result = service.BookCabana("W-0-0", new BookingRequest("102", "Bob"));

        Assert.False(result.Success);
        Assert.Equal(BookingError.AlreadyBooked, result.Error);
        Assert.Contains("already booked", result.Message);
    }

    [Fact]
    public void BookCabana_InvalidGuest_ReturnsInvalidGuestError()
    {
        var map = CreateMapWithCabanas((0, 0));
        var validator = Substitute.For<IGuestValidator>();
        validator.IsValid(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
        var service = new BookingService(validator, map);

        var result = service.BookCabana("W-0-0", new BookingRequest("999", "Fake Person"));

        Assert.False(result.Success);
        Assert.Equal(BookingError.InvalidGuest, result.Error);
    }

    [Fact]
    public void BookCabana_ValidRequest_UpdatesCabanaStatusToBooked()
    {
        var map = CreateMapWithCabanas((0, 0));
        var service = CreateService(map);

        service.BookCabana("W-0-0", new BookingRequest("101", "Alice Smith"));

        var cabana = service.GetAllCabanas().Single();
        Assert.Equal(CabanaStatus.Booked, cabana.Status);
    }

    [Fact]
    public void BookCabana_ValidatorReceivesCorrectArguments()
    {
        var map = CreateMapWithCabanas((0, 0));
        var validator = Substitute.For<IGuestValidator>();
        validator.IsValid("101", "Alice Smith").Returns(true);
        var service = new BookingService(validator, map);

        service.BookCabana("W-0-0", new BookingRequest("101", "Alice Smith"));

        validator.Received(1).IsValid("101", "Alice Smith");
    }
}
