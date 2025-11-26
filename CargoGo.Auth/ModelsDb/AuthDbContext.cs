using CargoGo.Auth.ModelsDb;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CargoGo.Auth;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<AppUser>(options);
