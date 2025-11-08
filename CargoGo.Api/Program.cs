using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using CargoGo.Api.Models;
using CargoGo.Auth;
using CargoGo.Dal;
using CargoGo.Dal.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container
builder.Services.AddControllers();
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

    // Добавляем схему авторизации JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
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
var appConnectionString = builder.Configuration.GetConnectionString("CargoGoConnection");
builder.Services.AddDbContext<CargoGoContext>(options =>
    options.UseSqlite(appConnectionString));

builder.Services.AddCargoGoIdentity(builder.Configuration);

var app = builder.Build();

/*
// Ensure database is created (temporary dev setup)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CargoGoContext>();
    db.Database.EnsureCreated();
}
*/

// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
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

app.UseHttpsRedirection();


app.MapControllers();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

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
    .ProducesValidationProblem()
    .RequireAuthorization();

app.Run();