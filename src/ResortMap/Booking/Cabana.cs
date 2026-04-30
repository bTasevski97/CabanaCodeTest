using ResortMap.Guests;

namespace ResortMap.Booking;

public enum CabanaStatus { Available, Booked }

public record Cabana(string Id, int Row, int Col, CabanaStatus Status = CabanaStatus.Available, Guest? BookedBy = null);

public record BookingRequest(string Room, string GuestName);

public record BookingResponse(bool Success, string Message);

public record CabanaDto(string Id, int Row, int Col, CabanaStatus Status);
