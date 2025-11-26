using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using CargoGo.Api.Requests;
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

// Настройка статики — ДО всего остального
app.UseDefaultFiles();
app.UseStaticFiles();

// CORS, Auth, AuthZ
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// API
app.MapControllers();

// Swagger (только для разработки, но оставим временно)
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CargoGo API v1");
        c.RoutePrefix = string.Empty;
    });
}

// SPA fallback — в самом конце
app.MapFallbackToFile("/index.html");

app.Run();