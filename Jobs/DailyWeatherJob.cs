using Quartz;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkyBot.Data;
using SkyBot.Localization;
using SkyBot.Services;
using SkyBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace SkyBot.Jobs;

[DisallowConcurrentExecution]
public class DailyWeatherJob : IJob
{
    private readonly ITelegramBotClient _bot;
    private readonly AppDbContext _db;
    private readonly IWeatherService _weatherService;
    private readonly ILogger<DailyWeatherJob> _logger;

    public DailyWeatherJob(
        ITelegramBotClient bot,
        AppDbContext db,
        IWeatherService weatherService,
        ILogger<DailyWeatherJob> logger)
    {
        _bot = bot;
        _db = db;
        _weatherService = weatherService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var currentHour = DateTime.UtcNow.Hour;
        _logger.LogInformation("DailyWeatherJob executing for hour {Hour} UTC", currentHour);

        var subscriptions = await _db.Subscriptions
            .Include(s => s.BotUser)
            .Where(s => s.ScheduledHour == currentHour)
            .ToListAsync();

        if (!subscriptions.Any())
        {
            _logger.LogInformation("No subscriptions scheduled for hour {Hour}", currentHour);
            return;
        }

        _logger.LogInformation("Sending daily weather to {Count} subscribers", subscriptions.Count);

        foreach (var subscription in subscriptions)
        {
            try
            {
                var user = subscription.BotUser;
                if (user == null || user.IsBanned) continue;

                var weather = await _weatherService.GetCurrentWeatherAsync(subscription.City);
                if (weather == null)
                {
                    _logger.LogWarning("City not found for subscription: {City}", subscription.City);
                    continue;
                }

                var weatherText = string.Format(
                    MessageResolver.Get(user, "WeatherResult"),
                    weather.CityName,
                    weather.Country,
                    weather.Temp,
                    weather.FeelsLike,
                    weather.Humidity,
                    weather.WindSpeed,
                    weather.Description,
                    weather.UpdatedAt.ToString("HH:mm"));

                var message = string.Format(MessageResolver.Get(user, "DailyReminderMessage"), weather.CityName, weatherText);

                await _bot.SendMessage(
                    subscription.TelegramUserId,
                    message);

                await Task.Delay(50);
            }
            catch (ApiRequestException apiEx) when (apiEx.ErrorCode == 403)
            {
                _logger.LogWarning("User {UserId} blocked the bot. Removing subscription.", subscription.TelegramUserId);
                await RemoveSubscriptionAsync(subscription.TelegramUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending daily weather to user {UserId}", subscription.TelegramUserId);
            }
        }
    }

    private async Task RemoveSubscriptionAsync(long telegramUserId)
    {
        var subscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.TelegramUserId == telegramUserId);
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
        }
    }
}
