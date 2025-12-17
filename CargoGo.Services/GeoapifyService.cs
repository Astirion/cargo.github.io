using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace CargoGo.Services;

public class GeoapifyService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public GeoapifyService(IConfiguration configuration, HttpClient httpClient)
    {
        _apiKey = configuration["Geoapify:ApiKey"] 
                  ?? throw new InvalidOperationException("Geoapify:ApiKey not configured");
        _httpClient = httpClient;
    }

    public async Task<JsonDocument> GeocodeAsync(string address)
    {
        var url = $"https://api.geoapify.com/v1/geocode/search?text={Uri.EscapeDataString(address)}&apiKey={_apiKey}"; // ✅ Убран пробел
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json);
    }

    public async Task<JsonDocument> ReverseGeocodeAsync(double lat, double lon)
    {
        var url = $"https://api.geoapify.com/v1/geocode/reverse?lat={lat}&lon={lon}&apiKey={_apiKey}"; // ✅ Убран пробел
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json);
    }

    public async Task<JsonDocument> GetIsolineAsync(double lat, double lon, int timeLimit = 600)
    {
        var url = $"https://api.geoapify.com/v1/isoline?lat={lat}&lon={lon}&type=drive&mode=shortest&timeLimit={timeLimit}&apiKey={_apiKey}"; // ✅ Убран пробел
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json);
    }
}