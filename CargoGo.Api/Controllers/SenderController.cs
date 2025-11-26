using System.Security.Claims;
using CargoGo.Api.Requests;
using CargoGo.Dal;
using CargoGo.Dal.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendersController(CargoGoContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Sender>>> GetSenders()
        {
            var items = await db.Senders
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new Sender
                {
                    Id = s.Id,
                    From = s.From,
                    To = s.To,
                    Weight = s.Weight,
                    Description = s.Description,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Sender>> CreateSender(Sender sender)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            
            var entity = new SenderEntity
            {
                From = sender.From,
                To = sender.To,
                Weight = sender.Weight,
                Description = sender.Description,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            db.Senders.Add(entity);
            await db.SaveChangesAsync();

            sender.Id = entity.Id;
            sender.CreatedAt = entity.CreatedAt;

            return Ok(sender);
        }
    }
}