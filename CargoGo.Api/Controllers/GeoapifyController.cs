using CargoGo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargoGo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeoapifyController : ControllerBase
{
    private readonly GeoapifyService _geoapify;

    public GeoapifyController(GeoapifyService geoapify)
    {
        _geoapify = geoapify;
    }

    [HttpGet("geocode")]
    public async Task<IActionResult> Geocode([FromQuery] string address)
    {
        try
        {
            var result = await _geoapify.GeocodeAsync(address);
            return Ok(result.RootElement);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("reverse")]
    public async Task<IActionResult> Reverse([FromQuery] double lat, [FromQuery] double lon)
    {
        try
        {
            var result = await _geoapify.ReverseGeocodeAsync(lat, lon);
            return Ok(result.RootElement);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}