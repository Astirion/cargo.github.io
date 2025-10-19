# CargoGo.Auth

Модуль авторизации на базе ASP.NET Core Identity.

## Использование

1. Добавьте зависимость на этот проект в ваш API.
2. В Program.cs зарегистрируйте Identity:

```csharp
using CargoGo.Auth;

builder.Services.AddCargoGoIdentity("Data Source=auth.db");
```

3. Примените миграции:

```
dotnet ef migrations add InitialIdentity -c AuthDbContext -p CargoGo.Auth/CargoGo.Auth.csproj
dotnet ef database update -c AuthDbContext -p CargoGo.Auth/CargoGo.Auth.csproj
```
