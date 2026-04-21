using ResortMap.Guests;

namespace ResortMap.Booking;

public enum CabanaStatus { Available, Booked }

public class Cabana
{
    public string Id { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Col { get; set; }
    public CabanaStatus Status { get; set; } = CabanaStatus.Available;
    public Guest? BookedBy { get; set; }
}

public record BookingRequest(string Room, string GuestName);

public record BookingResponse(bool Success, string Message);
