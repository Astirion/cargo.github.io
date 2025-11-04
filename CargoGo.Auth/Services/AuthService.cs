using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CargoGo.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace CargoGo.Auth.Services
{
    public class AuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AuthService> _logger;
        private readonly string _jwtKey;
        private readonly int _jwtExpires;

        public AuthService(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<AuthService> logger,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _jwtKey = configuration["Jwt:Key"];
            _jwtExpires = int.Parse(configuration["Jwt:ExpiryInMinutes"] ?? "60");
        }

        public async Task<bool> RegisterAsync(string login, string email, string password)
        {
            var user = new AppUser { UserName = login, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                _logger.LogInformation("Пользователь {Email} успешно зарегистрирован.", email);
                return true;
            }
            foreach (var error in result.Errors)
            {
                _logger.LogError("Ошибка регистрации: {Code} - {Description}", error.Code, error.Description);
            }
            return false;
        }

        public async Task<(bool success,  string? token, string? errorMessage)> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Пользователь с email {Email} не найден.", email);
                return (false, null, "Пользователь не найден.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, lockoutOnFailure: false, isPersistent: false);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("Вход для {Email} успешен.", email);
                var token = GenerateJwtToken(user);
                return (true, token, null);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("Пользователь {Email} заблокирован.", email);
                return (false, null, "Пользователь заблокирован.");
            }
            else if (result.RequiresTwoFactor)
            {
                _logger.LogWarning("Для пользователя {Email} требуется двухфакторная аутентификация.", email);
                return (false, null, "Требуется двухфакторная аутентификация.");
            }
            else if (result.IsNotAllowed)
            {
                _logger.LogWarning("Вход для пользователя {Email} не разрешен (например, email не подтвержден).", email);
                return (false, null, "Вход не разрешен.");
            }
            else
            {
                _logger.LogWarning("Неверный пароль для пользователя {Email}.", email);
                return (false, null, "Неверный пароль.");
            }
        }
        
        private string GenerateJwtToken(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtExpires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}