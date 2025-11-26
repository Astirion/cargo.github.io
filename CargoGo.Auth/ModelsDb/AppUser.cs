using Microsoft.AspNetCore.Identity;

namespace CargoGo.Auth.ModelsDb;

public class AppUser : IdentityUser
{
    public string? Telegram { get; set; }
}