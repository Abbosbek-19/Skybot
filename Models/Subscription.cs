namespace SkyBot.Models;

/// <summary>
/// Stores daily weather subscriptions.
/// Each user can have at most one active subscription.
/// Maps to the "Subscriptions" table.
/// </summary>
public class Subscription
{
    public int Id { get; set; }

    /// <summary>
    /// The Telegram user ID of the subscriber.
    /// </summary>
    public long TelegramUserId { get; set; }

    public string City { get; set; } = string.Empty;

    /// <summary>
    /// UTC hour (0–23) at which the daily weather should be sent.
    /// </summary>
    public int ScheduledHour { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public BotUser BotUser { get; set; } = null!;
}
