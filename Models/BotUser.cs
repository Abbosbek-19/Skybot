namespace SkyBot.Models;

/// <summary>
/// Represents a Telegram user who has started the bot.
/// Maps to the "BotUsers" table.
/// </summary>
public class BotUser
{
    public int Id { get; set; }

    /// <summary>
    /// The unique Telegram user ID (from Telegram API).
    /// </summary>
    public long TelegramUserId { get; set; }

    public string? Username { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }

    /// <summary>
    /// If true, this user can access /admin commands.
    /// </summary>
    public bool IsAdmin { get; set; } = false;

    /// <summary>
    /// If true, this user is blocked and the bot will not respond.
    /// </summary>
    public bool IsBanned { get; set; } = false;

    public bool IsSubscribed { get; set; } = false;
    public string? SubscribedCity { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<SearchHistory> SearchHistories { get; set; } = new List<SearchHistory>();
    public Subscription? Subscription { get; set; }
}
