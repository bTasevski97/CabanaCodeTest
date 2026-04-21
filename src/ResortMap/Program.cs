using ResortMap.Booking;
using ResortMap.Guests;
using ResortMap.Map;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder().AddCommandLine(args).Build();
string mapPath = config["map"] ?? "map.ascii";
string bookingsPath = config["bookings"] ?? "bookings.json";

if (!File.Exists(mapPath))
{
    Console.Error.WriteLine($"Error: map file '{mapPath}' not found.");
    Environment.Exit(1);
}


builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));

builder.Services.AddSingleton<MapResponse>(_ =>
    new MapParserService().ParseMap(File.ReadAllLines(mapPath)));

builder.Services.AddSingleton<IGuestValidator>(_ =>
{
    var validator = new GuestValidator();
    if (File.Exists(bookingsPath))
    {
        validator.Load(bookingsPath);
    }
    else
    {
        Console.WriteLine($"Warning: bookings file '{bookingsPath}' not found. Guest validation will fail.");
    }
    return validator;
});

builder.Services.AddSingleton<IBookingService, BookingService>();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/map", (MapResponse map) => Results.Ok(map));
app.MapGet("/api/cabanas", (IBookingService bookingSvc) => Results.Ok(bookingSvc.GetAllCabanas()));
app.MapPost("/api/cabanas/{id}/book", (string id, BookingRequest req, IBookingService bookingSvc) =>
    bookingSvc.BookCabana(id, req));

app.MapFallbackToFile("index.html");

app.Run();
