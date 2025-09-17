using CargoGo.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    c.DocumentTitle = "CargoGo API";
    c.DefaultModelsExpandDepth(-1); // Hide models section
});


app.UseCors();

var travelers = new List<Traveler>
{
    new Traveler { Id = 1, From = "Москва", To = "Санкт-Петербург", Weight = 15.5, Reward = 2500, CreatedAt = DateTime.UtcNow.AddHours(-2) },
    new Traveler { Id = 2, From = "Казань", To = "Екатеринбург", Weight = 8.0, Reward = 1800, CreatedAt = DateTime.UtcNow.AddHours(-1) },
    new Traveler { Id = 3, From = "Новосибирск", To = "Красноярск", Weight = 25.0, Reward = 3200, CreatedAt = DateTime.UtcNow.AddMinutes(-30) },
    new Traveler { Id = 4, From = "Ростов-на-Дону", To = "Сочи", Weight = 12.3, Reward = 2100, CreatedAt = DateTime.UtcNow.AddMinutes(-15) },
    new Traveler { Id = 5, From = "Владивосток", To = "Хабаровск", Weight = 30.0, Reward = 4500, CreatedAt = DateTime.UtcNow.AddMinutes(-5) }
};

var senders = new List<Sender>();
var travelerId = 6;
var senderId = 1;

/// <summary>
/// Получить список всех путешественников
/// </summary>
/// <returns>Список путешественников</returns>
app.MapGet("/api/travelers", () => Results.Ok(travelers))
    .WithName("GetTravelers");

/// <summary>
/// Добавить нового путешественника
/// </summary>
/// <param name="traveler">Данные путешественника</param>
/// <returns>Созданный путешественник</returns>
app.MapPost("/api/travelers", (Traveler traveler) =>
{
    traveler.Id = travelerId++;
    traveler.CreatedAt = DateTime.UtcNow;
    travelers.Add(traveler);
    return Results.Ok(traveler);
})
    .WithName("CreateTraveler");

/// <summary>
/// Получить список всех отправителей
/// </summary>
/// <returns>Список отправителей</returns>
app.MapGet("/api/senders", () => Results.Ok(senders))
    .WithName("GetSenders");

/// <summary>
/// Добавить нового отправителя
/// </summary>
/// <param name="sender">Данные отправителя</param>
/// <returns>Созданный отправитель</returns>
app.MapPost("/api/senders", (Sender sender) =>
{
    sender.Id = senderId++;
    sender.CreatedAt = DateTime.UtcNow;
    senders.Add(sender);
    return Results.Ok(sender);
})
    .WithName("CreateSender");

app.Run();


