using Microsoft.AspNetCore.Mvc;

namespace ResortMap.Booking;

[ApiController]
[Route("api/[controller]")]
public class CabanasController(IBookingService bookingService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(bookingService.GetAllCabanas());

    [HttpPost("{id}/bookings")]
    public IActionResult CreateBooking(string id, [FromBody] BookingRequest request)
    {
        var result = bookingService.BookCabana(id, request);

        if (!result.Success)
        {
            return result.Error switch
            {
                BookingError.NotFound => NotFound(new BookingResponse(false, result.Message)),
                BookingError.AlreadyBooked => Conflict(new BookingResponse(false, result.Message)),
                BookingError.InvalidGuest => BadRequest(new BookingResponse(false, result.Message)),
                _ => Problem(result.Message)
            };
        }

        return Created($"/api/cabanas/{id}/bookings", new BookingResponse(true, result.Message));
    }
}
