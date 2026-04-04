using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkyBot.Data;
using SkyBot.Models;

namespace SkyBot.Services;

/// <summary>
/// Handles all subscription-related database operations.
/// Injected into CommandHandler and DailyWeatherJob.
/// </summary>
public class SubscriptionService
{
    private readonly AppDbContext _db;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(AppDbContext db, ILogger<SubscriptionService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Creates or updates a subscription for the user.
    /// Returns false if the user is already subscribed to the same city at the same hour.
    /// </summary>
    public async Task<bool> SubscribeAsync(long telegramUserId, string city, int hour)
    {
        var existing = await _db.Subscriptions
            .FirstOrDefaultAsync(s => s.TelegramUserId == telegramUserId);

        if (existing != null)
        {
            // Silently update even if same city — allows changing hour
            existing.City = city;
            existing.ScheduledHour = hour;
            existing.CreatedAt = DateTime.UtcNow;
        }
        else
        {
            _db.Subscriptions.Add(new Subscription
            {
                TelegramUserId = telegramUserId,
                City = city,
                ScheduledHour = hour
            });
        }

        // Keep BotUser in sync
        var user = await _db.BotUsers.FirstOrDefaultAsync(u => u.TelegramUserId == telegramUserId);
        if (user != null)
        {
            user.IsSubscribed = true;
            user.SubscribedCity = city;
        }

        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Removes the subscription for the user.
    /// Returns false if the user had no subscription.
    /// </summary>
    public async Task<bool> UnsubscribeAsync(long telegramUserId)
    {
        var subscription = await _db.Subscriptions
            .FirstOrDefaultAsync(s => s.TelegramUserId == telegramUserId);

        if (subscription == null)
            return false;

        _db.Subscriptions.Remove(subscription);

        var user = await _db.BotUsers.FirstOrDefaultAsync(u => u.TelegramUserId == telegramUserId);
        if (user != null)
        {
            user.IsSubscribed = false;
            user.SubscribedCity = null;
        }

        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Returns the subscription for a user, or null if they have none.
    /// </summary>
    public async Task<Subscription?> GetSubscriptionAsync(long telegramUserId)
    {
        return await _db.Subscriptions
            .FirstOrDefaultAsync(s => s.TelegramUserId == telegramUserId);
    }

    /// <summary>
    /// Returns all subscriptions scheduled for the given UTC hour.
    /// Used by DailyWeatherJob every hour.
    /// </summary>
    public async Task<List<Subscription>> GetSubscriptionsForHourAsync(int hour)
    {
        return await _db.Subscriptions
            .Include(s => s.BotUser)
            .Where(s => s.ScheduledHour == hour)
            .ToListAsync();
    }

    /// <summary>
    /// Removes a subscription when the user has blocked the bot (Telegram 403 error).
    /// </summary>
    public async Task RemoveBlockedUserSubscriptionAsync(long telegramUserId)
    {
        var subscription = await _db.Subscriptions
            .FirstOrDefaultAsync(s => s.TelegramUserId == telegramUserId);

        if (subscription != null)
        {
            _db.Subscriptions.Remove(subscription);

            var user = await _db.BotUsers.FirstOrDefaultAsync(u => u.TelegramUserId == telegramUserId);
            if (user != null)
            {
                user.IsSubscribed = false;
                user.SubscribedCity = null;
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("Removed subscription for blocked user {UserId}", telegramUserId);
        }
    }

    /// <summary>
    /// Total count of active subscriptions (for admin stats).
    /// </summary>
    public async Task<int> GetTotalSubscriptionsCountAsync()
    {
        return await _db.Subscriptions.CountAsync();
    }
}
