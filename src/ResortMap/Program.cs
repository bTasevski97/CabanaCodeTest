using ResortMap.Booking;
using ResortMap.Guests;
using ResortMap.Maps;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder().AddCommandLine(args).Build();
string mapPath = config["map"] ?? "map.ascii";
string bookingsPath = config["bookings"] ?? "bookings.json";

try
{
    var map = Map.Parse(File.ReadAllLines(mapPath));
    builder.Services.AddSingleton(map);

    var guestValidator = new GuestValidator(bookingsPath);
    builder.Services.AddSingleton<IGuestValidator>(guestValidator);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Critical Startup Failure: {ex.Message}");
    Environment.Exit(1);
}

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));

builder.Services.AddSingleton<IBookingService, BookingService>();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(policy => 
    policy.WithOrigins("http://localhost:5173")
          .AllowAnyMethod()
          .AllowAnyHeader());

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

