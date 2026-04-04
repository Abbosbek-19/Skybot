using System.Text.Json.Serialization;

namespace SkyBot.DTOs;

// ─── Current Weather API response shapes ────────────────────────────────────
// Maps to: https://api.openweathermap.org/data/2.5/weather?q={city}&appid={key}&units=metric&lang=uz

public class WeatherApiDto
{
    [JsonPropertyName("name")]
    public string CityName { get; set; } = string.Empty;

    [JsonPropertyName("sys")]
    public WeatherSysDto Sys { get; set; } = new();

    [JsonPropertyName("main")]
    public WeatherMainDto Main { get; set; } = new();

    [JsonPropertyName("wind")]
    public WeatherWindDto Wind { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<WeatherDescriptionDto> Weather { get; set; } = new();

    [JsonPropertyName("dt")]
    public long Dt { get; set; }
}

public class WeatherSysDto
{
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
}

public class WeatherMainDto
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("temp_min")]
    public double TempMin { get; set; }

    [JsonPropertyName("temp_max")]
    public double TempMax { get; set; }
}

public class WeatherWindDto
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
}

public class WeatherDescriptionDto
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}

// ─── 3-Day Forecast API response shapes ─────────────────────────────────────
// Maps to: https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={key}&units=metric&lang=uz

public class ForecastApiDto
{
    [JsonPropertyName("city")]
    public ForecastCityDto City { get; set; } = new();

    [JsonPropertyName("list")]
    public List<ForecastItemDto> List { get; set; } = new();
}

public class ForecastCityDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
}

public class ForecastItemDto
{
    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    [JsonPropertyName("main")]
    public WeatherMainDto Main { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<WeatherDescriptionDto> Weather { get; set; } = new();

    [JsonPropertyName("dt_txt")]
    public string DtTxt { get; set; } = string.Empty;
}
