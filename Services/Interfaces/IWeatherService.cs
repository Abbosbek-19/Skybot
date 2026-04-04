using SkyBot.DTOs;

namespace SkyBot.Services.Interfaces;

/// <summary>
/// Defines the contract for fetching weather data from OpenWeatherMap.
/// Returns null when city is not found; throws on unexpected API errors.
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Fetches current weather for the given city.
    /// Returns null if the city doesn't exist.
    /// </summary>
    Task<WeatherResponseDto?> GetCurrentWeatherAsync(string city);

    /// <summary>
    /// Fetches 3-day forecast for the given city.
    /// Returns null if the city doesn't exist.
    /// Key = date label (e.g. "4-aprel, Juma")
    /// Value = one ForecastItemDto representing that day's noon reading.
    /// </summary>
    Task<(string CityName, List<ForecastItemDto> Days)?> GetForecastAsync(string city);
}
