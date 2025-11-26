using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CargoGo.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RegisterRequest = CargoGo.Api.Requests.Auth.RegisterRequest;

namespace CargoGo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Register")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Login) || string.IsNullOrWhiteSpace(req.Password) || string.IsNullOrWhiteSpace(req.Email))
        {
            return BadRequest(new { message = "Все поля обязательны" });
        }
        
        var result = await _authService.RegisterAsync( req.Login,  req.Email, req.Password);

        if (result.success)
            return Ok();

        return BadRequest(result.errorMessage);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var (success, token, errorMessage) = await _authService.LoginAsync(model.Email, model.Password);
        if (success)
            return Ok(token);
        return Unauthorized(errorMessage);
    }
    
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _authService.FindByIdAsync(userId);
        if (user == null) return NotFound();

        return Ok(new
        {
            name = user.NormalizedUserName ?? user.UserName,
            email = user.Email,
            phoneNumber = user.PhoneNumber ?? "Не указан",
            telegram = user.Telegram ?? "Не указан" 
        });
    }
}
