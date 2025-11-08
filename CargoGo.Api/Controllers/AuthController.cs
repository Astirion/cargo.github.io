using System.ComponentModel.DataAnnotations;
using CargoGo.Auth.Services;
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

        if (result)
            return Ok();

        return BadRequest("Ошибка регистрации");
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var (success, token, errorMessage) = await _authService.LoginAsync(model.Email, model.Password);
        if (success)
            return Ok(token);
        return Unauthorized(errorMessage);
    }
}
