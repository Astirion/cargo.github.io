
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CargoGo.Api.Models;
using CargoGo.Dal;
using CargoGo.Dal.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

// DAL: Register DbContext with SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? "Data Source=cargogo.db";
builder.Services.AddDbContext<CargoGoContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// Ensure database is created (temporary dev setup)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CargoGoContext>();
    db.Database.EnsureCreated();
}

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

// In-memory data removed. Using SQLite via CargoGoContext.

app.MapGet("/api/travelers", (CargoGoContext db) =>
{
    var items = db.Travelers
        .OrderByDescending(t => t.CreatedAt)
        .Select(t => new Traveler
        {
            Id = t.Id,
            From = t.From,
            To = t.To,
            Weight = t.Weight,
            Reward = t.Reward,
            CreatedAt = t.CreatedAt
        })
        .ToList();
    return Results.Ok(items);
})
    .WithName("GetTravelers")
    .WithTags("Travelers")
    .Produces<List<Traveler>>(StatusCodes.Status200OK);

app.MapPost("/api/travelers", (CargoGoContext db, Traveler traveler) =>
{
    var entity = new TravelerEntity
    {
        From = traveler.From,
        To = traveler.To,
        Weight = traveler.Weight,
        Reward = traveler.Reward,
        CreatedAt = DateTime.UtcNow
    };
    db.Travelers.Add(entity);
    db.SaveChanges();

    traveler.Id = entity.Id;
    traveler.CreatedAt = entity.CreatedAt;
    return Results.Ok(traveler);
})
    .WithName("CreateTraveler")
    .WithTags("Travelers")
    .Accepts<Traveler>("application/json")
    .Produces<Traveler>(StatusCodes.Status200OK)
    .ProducesValidationProblem();

app.MapGet("/api/senders", (CargoGoContext db) =>
{
    var items = db.Senders
        .OrderByDescending(s => s.CreatedAt)
        .Select(s => new Sender
        {
            Id = s.Id,
            From = s.From,
            To = s.To,
            Weight = s.Weight,
            Description = s.Description,
            CreatedAt = s.CreatedAt
        })
        .ToList();
    return Results.Ok(items);
})
    .WithName("GetSenders")
    .WithTags("Senders")
    .Produces<List<Sender>>(StatusCodes.Status200OK);

app.MapPost("/api/senders", (CargoGoContext db, Sender sender) =>
{
    var entity = new SenderEntity
    {
        From = sender.From,
        To = sender.To,
        Weight = sender.Weight,
        Description = sender.Description,
        CreatedAt = DateTime.UtcNow
    };
    db.Senders.Add(entity);
    db.SaveChanges();

    sender.Id = entity.Id;
    sender.CreatedAt = entity.CreatedAt;
    return Results.Ok(sender);
})
    .WithName("CreateSender")
    .WithTags("Senders")
    .Accepts<Sender>("application/json")
    .Produces<Sender>(StatusCodes.Status200OK)
    .ProducesValidationProblem();

app.Run();


