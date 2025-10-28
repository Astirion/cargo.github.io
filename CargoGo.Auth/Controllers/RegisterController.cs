using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CargoGo.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Register")]
public class RegisterController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    public RegisterController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Login) || string.IsNullOrWhiteSpace(req.Password) || string.IsNullOrWhiteSpace(req.Email))
        {
            return BadRequest(new { message = "Все поля обязательны" });
        }

        var user = new IdentityUser
        {
            UserName = req.Login,
            Email = req.Email
        };

        var result = await _userManager.CreateAsync(user, req.Password);

        if (result.Succeeded)
            return Ok();

        var errorMsg = string.Join("; ", result.Errors.Select(e => e.Description));
        return BadRequest(new { message = errorMsg });
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
