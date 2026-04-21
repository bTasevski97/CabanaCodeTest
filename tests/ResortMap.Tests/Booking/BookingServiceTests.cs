using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using ResortMap.Booking;
using ResortMap.Guests;
using ResortMap.Map;
using Xunit;

namespace ResortMap.Tests.Booking;

public class BookingServiceTests
{
    private static MapResponse CreateMapWithCabanas(params (int row, int col)[] positions)
    {
        var map = new MapResponse
        {
            Rows = 5,
            Cols = 5,
            Tiles = []
        };

        foreach (var (row, col) in positions)
        {
            map.Tiles.Add(new Tile
            {
                Type = TileType.Cabana,
                Row = row,
                Col = col,
                CabanaId = $"W-{row}-{col}"
            });
        }

        return map;
    }

    private static BookingService CreateService(MapResponse map, IGuestValidator? validator = null)
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
        var map = new MapResponse
        {
            Rows = 1,
            Cols = 3,
            Tiles =
            [
                new Tile { Type = TileType.Path, Row = 0, Col = 0 },
                new Tile { Type = TileType.Cabana, Row = 0, Col = 1, CabanaId = "W-0-1" },
                new Tile { Type = TileType.Pool, Row = 0, Col = 2 },
            ]
        };
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
    public void BookCabana_ValidRequest_ReturnsOkWithSuccessMessage()
    {
        var map = CreateMapWithCabanas((0, 0));
        var service = CreateService(map);

        var result = service.BookCabana("W-0-0", new BookingRequest("101", "Alice Smith"));

        var ok = Assert.IsType<Ok<BookingResponse>>(result);
        Assert.True(ok.Value!.Success);
        Assert.Contains("Alice Smith", ok.Value.Message);
        Assert.Contains("101", ok.Value.Message);
    }

    [Fact]
    public void BookCabana_NonExistentId_ReturnsNotFound()
    {
        var map = CreateMapWithCabanas((0, 0));
        var service = CreateService(map);

        var result = service.BookCabana("W-99-99", new BookingRequest("101", "Alice"));

        var notFound = Assert.IsType<NotFound<BookingResponse>>(result);
        Assert.False(notFound.Value!.Success);
    }

    [Fact]
    public void BookCabana_AlreadyBooked_ReturnsConflict()
    {
        var map = CreateMapWithCabanas((0, 0));
        var service = CreateService(map);

        service.BookCabana("W-0-0", new BookingRequest("101", "Alice"));
        var result = service.BookCabana("W-0-0", new BookingRequest("102", "Bob"));

        var conflict = Assert.IsType<Conflict<BookingResponse>>(result);
        Assert.False(conflict.Value!.Success);
        Assert.Contains("already booked", conflict.Value.Message);
    }

    [Fact]
    public void BookCabana_InvalidGuest_ReturnsBadRequest()
    {
        var map = CreateMapWithCabanas((0, 0));
        var validator = Substitute.For<IGuestValidator>();
        validator.IsValid(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
        var service = new BookingService(validator, map);

        var result = service.BookCabana("W-0-0", new BookingRequest("999", "Fake Person"));

        var badRequest = Assert.IsType<BadRequest<BookingResponse>>(result);
        Assert.False(badRequest.Value!.Success);
    }

    [Fact]
    public void BookCabana_ValidRequest_UpdatesCabanaState()
    {
        var map = CreateMapWithCabanas((0, 0));
        var service = CreateService(map);

        service.BookCabana("W-0-0", new BookingRequest("101", "Alice Smith"));

        var cabana = service.GetAllCabanas().Single();
        Assert.Equal(CabanaStatus.Booked, cabana.Status);
        Assert.NotNull(cabana.BookedBy);
        Assert.Equal("101", cabana.BookedBy.Room);
        Assert.Equal("Alice Smith", cabana.BookedBy.GuestName);
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
