using System.Net;
using System.Text.Json;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SkyBot.DTOs;
using SkyBot.Services.Interfaces;

namespace SkyBot.Services;

/// <summary>
/// Fetches weather data from OpenWeatherMap REST API.
/// Named HttpClient "WeatherClient" is registered in Program.cs.
/// All results are in metric units; language=uz for Uzbek descriptions.
/// </summary>
public class WeatherService : IWeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<WeatherService> _logger;

    private static readonly Dictionary<string, string> WeatherTranslations = new(StringComparer.OrdinalIgnoreCase)
    {
        { "clear sky", "osmon ochiq" },
        { "few clouds", "kam bulutli" },
        { "scattered clouds", "tarqoq bulutli" },
        { "broken clouds", "bulutli" },
        { "overcast clouds", "qoplamali bulutli" },
        { "shower rain", "qisqa muddatli yomg'ir" },
        { "rain", "yomg'ir" },
        { "light rain", "engil yomg'ir" },
        { "moderate rain", "o'rtacha yomg'ir" },
        { "heavy intensity rain", "kuchli yomg'ir" },
        { "thunderstorm", "mo'maqaldiroq" },
        { "snow", "qor" },
        { "mist", "tuman" },
        { "haze", "shovqin" },
        { "fog", "tuman" },
        { "dust", "chang" },
        { "sand", "qum" },
        { "smoke", "tutun" },
        { "drizzle", "mayda yomg'ir" }
    };

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static string TranslateCondition(string condition)
    {
        if (string.IsNullOrEmpty(condition)) return condition;
        return WeatherTranslations.TryGetValue(condition, out var translation) ? translation : condition;
    }

    public WeatherService(
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        ILogger<WeatherService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
    }

    public async Task<WeatherResponseDto?> GetCurrentWeatherAsync(string city)
    {
        var apiKey = _config["WeatherSettings:ApiKey"]!;
        var baseUrl = _config["WeatherSettings:BaseUrl"]!;
        var url = $"{baseUrl}weather?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric&lang=uz";

        try
        {
            var client = _httpClientFactory.CreateClient("WeatherClient");
            var response = await client.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null; // City doesn't exist — caller will show CityNotFound message

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<WeatherApiDto>(json, _jsonOptions);

            if (data == null) return null;

            return new WeatherResponseDto
            {
                CityName    = data.CityName,
                Country     = data.Sys.Country,
                Temp        = Math.Round(data.Main.Temp, 1),
                FeelsLike   = Math.Round(data.Main.FeelsLike, 1),
                Humidity    = data.Main.Humidity,
                WindSpeed   = Math.Round(data.Wind.Speed, 1),
                Description = TranslateCondition(CapitalizeFirst(data.Weather.FirstOrDefault()?.Description ?? "")),
                UpdatedAt   = DateTimeOffset.FromUnixTimeSeconds(data.Dt).UtcDateTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current weather for city: {City}", city);
            throw; // Let the handler catch and show WeatherError to user
        }
    }

    public async Task<(string CityName, List<ForecastItemDto> Days)?> GetForecastAsync(string city)
    {
        var apiKey = _config["WeatherSettings:ApiKey"]!;
        var baseUrl = _config["WeatherSettings:BaseUrl"]!;
        // cnt=24 gives us readings every 3 hours for 3 days (8 per day × 3 = 24)
        var url = $"{baseUrl}forecast?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric&lang=uz&cnt=24";

        try
        {
            var client = _httpClientFactory.CreateClient("WeatherClient");
            var response = await client.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ForecastApiDto>(json, _jsonOptions);

            if (data == null) return null;

            // Group by day, pick the noon (12:00) entry for each day, take 3 days
            var days = data.List
                .GroupBy(item => DateTimeOffset.FromUnixTimeSeconds(item.Dt).UtcDateTime.Date)
                .Take(3)
                .Select(g =>
                {
                    // Prefer the 12:00 UTC entry; fallback to the first entry of the day
                    var noon = g.FirstOrDefault(i =>
                        DateTimeOffset.FromUnixTimeSeconds(i.Dt).UtcDateTime.Hour == 12)
                        ?? g.First();
                    return noon;
                })
                .ToList();

            return (data.City.Name, days);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching forecast for city: {City}", city);
            throw;
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static string CapitalizeFirst(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return char.ToUpperInvariant(text[0]) + text[1..];
    }
}
