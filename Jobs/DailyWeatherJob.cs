using Quartz;
using Microsoft.Extensions.Logging;
using SkyBot.Localization;
using SkyBot.Services;
using SkyBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace SkyBot.Jobs;

/// <summary>
/// Quartz.NET job that fires every hour.
/// Queries all subscriptions scheduled for the current UTC hour,
/// fetches live weather, and sends the result to each subscriber in Uzbek.
/// If the user has blocked the bot (403), their subscription is auto-deleted.
/// </summary>
[DisallowConcurrentExecution] // Prevents the job from running twice if it takes longer than 1 hour
public class DailyWeatherJob : IJob
{
    private readonly ITelegramBotClient _bot;
    private readonly SubscriptionService _subscriptionService;
    private readonly IWeatherService _weatherService;
    private readonly ILogger<DailyWeatherJob> _logger;

    public DailyWeatherJob(
        ITelegramBotClient bot,
        SubscriptionService subscriptionService,
        IWeatherService weatherService,
        ILogger<DailyWeatherJob> logger)
    {
        _bot = bot;
        _subscriptionService = subscriptionService;
        _weatherService = weatherService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var currentHour = DateTime.UtcNow.Hour;
        _logger.LogInformation("DailyWeatherJob executing for hour {Hour} UTC", currentHour);

        var subscriptions = await _subscriptionService.GetSubscriptionsForHourAsync(currentHour);

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
                var weather = await _weatherService.GetCurrentWeatherAsync(subscription.City);

                if (weather == null)
                {
                    _logger.LogWarning("City not found for subscription: {City}", subscription.City);
                    continue;
                }

                // Build the weather text in Uzbek
                var weatherText = string.Format(
                    UzMessages.WeatherResult,
                    weather.CityName,
                    weather.Country,
                    weather.Temp,
                    weather.FeelsLike,
                    weather.Humidity,
                    weather.WindSpeed,
                    weather.Description,
                    weather.UpdatedAt.ToString("HH:mm"));

                var message = string.Format(UzMessages.DailyWeatherMessage, weather.CityName, weatherText);

                await _bot.SendMessage(
                    subscription.TelegramUserId,
                    message,
                    parseMode: ParseMode.Markdown);

                // Small delay to respect Telegram rate limits (~30 msg/sec)
                await Task.Delay(50);
            }
            catch (ApiRequestException apiEx) when (apiEx.ErrorCode == 403)
            {
                // User has blocked the bot — remove their subscription automatically
                _logger.LogWarning("User {UserId} blocked the bot. Removing subscription.", subscription.TelegramUserId);
                await _subscriptionService.RemoveBlockedUserSubscriptionAsync(subscription.TelegramUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending daily weather to user {UserId}", subscription.TelegramUserId);
            }
        }
    }
}
