using ResortMap.Guests;
using ResortMap.Maps;

namespace ResortMap.Booking;

public enum BookingError { NotFound, AlreadyBooked, InvalidGuest }

public interface IBookingService
{
    IEnumerable<CabanaDto> GetAllCabanas();
    BookingResult BookCabana(string id, BookingRequest request);
}

public class BookingService(IGuestValidator guestValidator, Map map) : IBookingService
{
    private readonly Dictionary<string, Cabana> _cabanas = map.Tiles
        .Where(t => t.Type == TileType.Cabana && !string.IsNullOrEmpty(t.CabanaId))
        .ToDictionary(
            t => t.CabanaId!,
            t => new Cabana(t.CabanaId!, t.Row, t.Col)
        );

    public IEnumerable<CabanaDto> GetAllCabanas() => 
        _cabanas.Values.Select(c => new CabanaDto(c.Id, c.Row, c.Col, c.Status));

    public BookingResult BookCabana(string id, BookingRequest request)
    {
        if (!_cabanas.TryGetValue(id, out var cabana))
        {
            return BookingResult.Fail(BookingError.NotFound, "Cabana not found.");
        }

        if (cabana.Status == CabanaStatus.Booked)
        {
            return BookingResult.Fail(BookingError.AlreadyBooked, "This cabana is already booked.");
        }

        if (!guestValidator.IsValid(request.Room, request.GuestName))
        {
            return BookingResult.Fail(BookingError.InvalidGuest, "Invalid room number or guest name.");
        }

        _cabanas[id] = cabana with { Status = CabanaStatus.Booked, BookedBy = new Guest(request.Room, request.GuestName) };

        return BookingResult.Ok($"Cabana booked for {request.GuestName} (Room {request.Room}).");
    }
}

public class BookingResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public BookingError? Error { get; init; }

    public static BookingResult Ok(string message) => new() { Success = true, Message = message };
    public static BookingResult Fail(BookingError error, string message) => new() { Success = false, Error = error, Message = message };
}
