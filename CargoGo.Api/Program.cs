var builder = WebApplication.CreateBuilder(args);

app.UseCors(policy => policy
    .AllowAnyOrigin()  // Разрешить любой домен (для разработки)
    .AllowAnyMethod()
    .AllowAnyHeader());

app.Run();



