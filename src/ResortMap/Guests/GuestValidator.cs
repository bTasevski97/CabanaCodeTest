using System.Text.Json;

namespace ResortMap.Guests;

public record Guest(string Room, string GuestName);

public interface IGuestValidator
{
    bool IsValid(string room, string guestName);
}

public class GuestValidator : IGuestValidator
{
    private readonly HashSet<Guest> _guests = new();

    public void Load(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _guests.UnionWith(JsonSerializer.Deserialize<List<Guest>>(json, options) ?? []);
    }

    public bool IsValid(string room, string guestName)
    {
        return _guests.Contains(new Guest(room, guestName));
    }
}
