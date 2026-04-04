namespace SkyBot.Models;

/// <summary>
/// Stores every city search a user performs.
/// Maps to the "SearchHistories" table.
/// </summary>
public class SearchHistory
{
    public int Id { get; set; }

    /// <summary>
    /// The Telegram user ID of who made this search.
    /// </summary>
    public long TelegramUserId { get; set; }

    public string CityName { get; set; } = string.Empty;

    /// <summary>
    /// Short summary of the weather result stored at search time.
    /// </summary>
    public string WeatherSummary { get; set; } = string.Empty;

    public DateTime SearchedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public BotUser BotUser { get; set; } = null!;
}
