using System.ComponentModel.DataAnnotations;

namespace CargoGo.Api.Requests.Auth;

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