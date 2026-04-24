using System.Collections.Concurrent;
using ResortMap.Guests;
using ResortMap.Map;

namespace ResortMap.Booking;

public interface IBookingService
{
    IEnumerable<CabanaDto> GetAllCabanas();
    IResult BookCabana(string id, BookingRequest request);
}

public class BookingService : IBookingService
{
    private readonly ConcurrentDictionary<string, Cabana> _cabanas = new();
    private readonly IGuestValidator _guestValidator;

    public BookingService(IGuestValidator guestValidator, MapResponse map)
    {
        _guestValidator = guestValidator;

        foreach (var tile in map.Tiles)
        {
            if (tile.Type == TileType.Cabana && !string.IsNullOrEmpty(tile.CabanaId))
            {
                _cabanas.TryAdd(tile.CabanaId, new Cabana
                {
                    Id = tile.CabanaId,
                    Row = tile.Row,
                    Col = tile.Col,
                    Status = CabanaStatus.Available
                });
            }
        }
    }

    public IEnumerable<CabanaDto> GetAllCabanas() => 
        _cabanas.Values.Select(c => new CabanaDto(c.Id, c.Row, c.Col, c.Status));

    public IResult BookCabana(string id, BookingRequest request)
    {
        if (!_cabanas.TryGetValue(id, out var cabana))
        {
            return Results.NotFound(new BookingResponse(false, "Cabana not found."));
        }

        lock (cabana)
        {
            if (cabana.Status == CabanaStatus.Booked)
            {
                return Results.Conflict(new BookingResponse(false, "This cabana is already booked."));
            }

            if (!_guestValidator.IsValid(request.Room, request.GuestName))
            {
                return Results.BadRequest(new BookingResponse(false, "Invalid room number or guest name."));
            }

            cabana.Status = CabanaStatus.Booked;
            cabana.BookedBy = new Guest(request.Room, request.GuestName);

            return Results.Ok(new BookingResponse(true, $"Cabana booked for {request.GuestName} (Room {request.Room})."));
        }
    }
}
