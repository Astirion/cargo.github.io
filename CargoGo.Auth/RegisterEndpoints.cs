using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CargoGo.Auth;

public static class RegisterEndpoints
{
    public static void MapRegisterEndpoint(this WebApplication app)
    {
        app.MapPost("/api/register", async (
            UserManager<IdentityUser> userManager,
            RegisterRequest req) =>
        {
            if (string.IsNullOrWhiteSpace(req.Login) || string.IsNullOrWhiteSpace(req.Password) || string.IsNullOrWhiteSpace(req.Email))
            {
                return Results.BadRequest(new { message = "Все поля обязательны" });
            }

            var user = new IdentityUser
            {
                UserName = req.Login,
                Email = req.Email
            };

            var result = await userManager.CreateAsync(user, req.Password);

            if (result.Succeeded)
                return Results.Ok();

            var errorMsg = string.Join("; ", result.Errors.Select(e => e.Description));
            return Results.BadRequest(new { message = errorMsg });
        })
        .WithName("RegisterUser")
        .WithTags("Auth")
        .Accepts<RegisterRequest>("application/json")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }

    public class RegisterRequest
    {
        [Required]
        public string Login { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
