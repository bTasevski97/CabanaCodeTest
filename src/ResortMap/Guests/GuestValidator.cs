using System.Text.Json;

namespace ResortMap.Guests;

public record Guest(string Room, string GuestName);

public interface IGuestValidator
{
    bool IsValid(string room, string guestName);
}

public class GuestValidator(string filePath) : IGuestValidator
{
    private readonly HashSet<Guest> _guests = LoadGuests(filePath);

    private static HashSet<Guest> LoadGuests(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Critical configuration missing: Guest bookings file '{path}' not found.");
        }

        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<HashSet<Guest>>(json, options) ?? new HashSet<Guest>();
    }

    public bool IsValid(string room, string guestName) => _guests.Contains(new Guest(room, guestName));
}
