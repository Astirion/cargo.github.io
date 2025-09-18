using CargoGo.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CargoGo API",
        Version = "v1",
        Description = "API для платформы CargoGo - сервиса поиска попутного груза и путешественников",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "CargoGo Support",
            Email = "support@cargogo.com"
        }
    });

    // Включаем XML комментарии
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Настройка тегов для группировки endpoints
    c.TagActionsBy(api => new[] { api.GroupName ?? "Default" });
    c.DocInclusionPredicate((name, api) => true);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CargoGo API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        c.DocumentTitle = "CargoGo API Documentation";
        c.DefaultModelsExpandDepth(2); // Show models section
        c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
        c.EnableValidator();
    });
}


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

app.MapGet("/api/travelers", () => Results.Ok(travelers))
    .WithName("GetTravelers")
    .WithTags("Travelers")
    .Produces<List<Traveler>>(StatusCodes.Status200OK);

app.MapPost("/api/travelers", (Traveler traveler) =>
{
    traveler.Id = travelerId++;
    traveler.CreatedAt = DateTime.UtcNow;
    travelers.Add(traveler);
    return Results.Ok(traveler);
})
    .WithName("CreateTraveler")
    .WithTags("Travelers")
    .Accepts<Traveler>("application/json")
    .Produces<Traveler>(StatusCodes.Status200OK)
    .ProducesValidationProblem();

app.MapGet("/api/senders", () => Results.Ok(senders))
    .WithName("GetSenders")
    .WithTags("Senders")
    .Produces<List<Sender>>(StatusCodes.Status200OK);

app.MapPost("/api/senders", (Sender sender) =>
{
    sender.Id = senderId++;
    sender.CreatedAt = DateTime.UtcNow;
    senders.Add(sender);
    return Results.Ok(sender);
})
    .WithName("CreateSender")
    .WithTags("Senders")
    .Accepts<Sender>("application/json")
    .Produces<Sender>(StatusCodes.Status200OK)
    .ProducesValidationProblem();

app.Run();


