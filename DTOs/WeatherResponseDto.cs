namespace SkyBot.DTOs;

/// <summary>
/// Clean internal representation of weather data after parsing the API response.
/// This is what WeatherService returns — never expose raw API DTOs to handlers.
/// </summary>
public class WeatherResponseDto
{
    public string CityName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Temp { get; set; }
    public double FeelsLike { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Short summary used when storing search history in DB.
    /// Example: "23°C, Qisman bulutli"
    /// </summary>
    public string ShortSummary => $"{Math.Round(Temp)}°C, {Description}";
}
