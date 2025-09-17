var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors();

var travelers = new List<Traveler>();
var senders = new List<Sender>();
var travelerId = 1;
var senderId = 1;

app.MapGet("/api/travelers", () => Results.Ok(travelers));
app.MapPost("/api/travelers", (Traveler traveler) =>
{
    traveler.Id = travelerId++;
    traveler.CreatedAt = DateTime.UtcNow;
    travelers.Add(traveler);
    return Results.Ok(traveler);
});

app.MapGet("/api/senders", () => Results.Ok(senders));
app.MapPost("/api/senders", (Sender sender) =>
{
    sender.Id = senderId++;
    sender.CreatedAt = DateTime.UtcNow;
    senders.Add(sender);
    return Results.Ok(sender);
});

app.Run();

public class Traveler
{
    public int Id { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public double Weight { get; set; }
    public int Reward { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Sender
{
    public int Id { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public double Weight { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

