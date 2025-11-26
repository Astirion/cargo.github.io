using System.Security.Claims;
using CargoGo.Api.Requests;
using CargoGo.Dal;
using CargoGo.Dal.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CargoGo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelersController(CargoGoContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Traveler>>> GetTravelers()
        {
            var items = await db.Travelers
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new Traveler
                {
                    Id = t.Id,
                    From = t.From,
                    To = t.To,
                    Weight = t.Weight,
                    Reward = t.Reward,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Traveler>> CreateTraveler(Traveler traveler)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            
            var entity = new TravelerEntity
            {
                TravelTime = traveler.TravelTime,
                From = traveler.From,
                To = traveler.To,
                Weight = traveler.Weight,
                Reward = traveler.Reward,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            db.Travelers.Add(entity);
            await db.SaveChangesAsync();

            traveler.Id = entity.Id;
            traveler.CreatedAt = entity.CreatedAt;

            return Ok(traveler);
        }
    }
}