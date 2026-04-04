using Microsoft.EntityFrameworkCore;
using SkyBot.Data;
using SkyBot.DTOs;
using SkyBot.Keyboards;
using SkyBot.Localization;
using SkyBot.Models;
using SkyBot.Services;
using SkyBot.Services.Interfaces;
using System.Globalization;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SkyBot.Handlers;

/// <summary>
/// Handles all slash-command messages.
/// Key public helpers (SendWeatherAsync, SendForecastAsync, SendHistoryAsync, SendMyStatsAsync)
/// are reused by CallbackQueryHandler so there's no duplicated logic.
/// </summary>
public class CommandHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly AppDbContext _db;
    private readonly IWeatherService _weatherService;
    private readonly SubscriptionService _subscriptionService;
    private readonly UserStateService _stateService;
    private readonly ILogger<CommandHandler> _logger;

    public CommandHandler(
        ITelegramBotClient bot,
        AppDbContext db,
        IWeatherService weatherService,
        SubscriptionService subscriptionService,
        UserStateService stateService,
        ILogger<CommandHandler> logger)
    {
        _bot = bot;
        _db = db;
        _weatherService = weatherService;
        _subscriptionService = subscriptionService;
        _stateService = stateService;
        _logger = logger;
    }

    // ─────────────────────────────────────────────────────────────────────
    //  Entry point — called from UpdateHandler
    // ─────────────────────────────────────────────────────────────────────

    public async Task HandleAsync(Message message, BotUser user)
    {
        var fullText = message.Text ?? string.Empty;
        // Strip "@BotName" suffix, e.g. "/start@SkyBot" → "/start"
        var command = fullText.Split(' ')[0].Split('@')[0].ToLowerInvariant();
        var args    = fullText.Contains(' ')
            ? fullText[(fullText.IndexOf(' ') + 1)..].Trim()
            : string.Empty;

        switch (command)
        {
            case "/start":       await HandleStart(message, user);             break;
            case "/help":        await HandleHelp(message.Chat.Id);            break;
            case "/weather":     await HandleWeather(message.Chat.Id, user, args); break;
            case "/forecast":    await HandleForecast(message.Chat.Id, user, args); break;
            case "/subscribe":   await HandleSubscribe(message.Chat.Id, user, args); break;
            case "/unsubscribe": await HandleUnsubscribe(message.Chat.Id, user);  break;
            case "/history":     await SendHistoryAsync(message.Chat.Id, user);   break;
            case "/mystats":     await SendMyStatsAsync(message.Chat.Id, user);   break;
            case "/admin":       await HandleAdmin(message.Chat.Id, user);        break;
            default:
                await _bot.SendMessage(message.Chat.Id, UzMessages.UnknownCommand,
                    parseMode: ParseMode.Markdown);
                break;
        }
    }

    // ─────────────────────────────────────────────────────────────────────
    //  /start
    // ─────────────────────────────────────────────────────────────────────

    private async Task HandleStart(Message message, BotUser user)
    {
        // If registered less than 5 seconds ago it's a truly new user
        var isNew = (DateTime.UtcNow - user.RegisteredAt).TotalSeconds < 5;
        var text = isNew
            ? string.Format(UzMessages.Welcome, user.FirstName)
            : string.Format(UzMessages.AlreadyRegistered, user.FirstName);

        await _bot.SendMessage(message.Chat.Id, text,
            parseMode: ParseMode.Markdown,
            replyMarkup: BotKeyboards.MainMenu());
    }

    // ─────────────────────────────────────────────────────────────────────
    //  /help
    // ─────────────────────────────────────────────────────────────────────

    private async Task HandleHelp(long chatId)
    {
        await _bot.SendMessage(chatId, UzMessages.HelpText, parseMode: ParseMode.Markdown);
    }

    // ─────────────────────────────────────────────────────────────────────
    //  /weather
    // ─────────────────────────────────────────────────────────────────────

    private async Task HandleWeather(long chatId, BotUser user, string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            _stateService.SetState(user.TelegramUserId, "awaiting_weather_city");
            await _bot.SendMessage(chatId, UzMessages.EnterCityName);
            return;
        }
        await SendWeatherAsync(chatId, user, city);
    }

    // ─────────────────────────────────────────────────────────────────────
    //  /forecast
    // ─────────────────────────────────────────────────────────────────────

    private async Task HandleForecast(long chatId, BotUser user, string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            _stateService.SetState(user.TelegramUserId, "awaiting_forecast_city");
            await _bot.SendMessage(chatId, UzMessages.EnterCityName);
            return;
        }
        await SendForecastAsync(chatId, user, city);
    }

    // ─────────────────────────────────────────────────────────────────────
    //  /subscribe
    // ─────────────────────────────────────────────────────────────────────

    private async Task HandleSubscribe(long chatId, BotUser user, string city)
    {
        var existing = await _subscriptionService.GetSubscriptionAsync(user.TelegramUserId);
        if (existing != null && string.IsNullOrWhiteSpace(city))
        {
            await _bot.SendMessage(chatId,
                string.Format(UzMessages.AlreadySubscribed, existing.City),
                parseMode: ParseMode.Markdown);
            return;
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            _stateService.SetState(user.TelegramUserId, "awaiting_subscribe_city");
            await _bot.SendMessage(chatId, UzMessages.SubscribeEnterCity);
            return;
        }

        await _bot.SendMessage(chatId, UzMessages.SubscribeSelectHour,
            replyMarkup: BotKeyboards.SubscribeHourPicker(city));
    }

    // ─────────────────────────────────────────────────────────────────────
    //  /unsubscribe
    // ─────────────────────────────────────────────────────────────────────

    private async Task HandleUnsubscribe(long chatId, BotUser user)
    {
        var removed = await _subscriptionService.UnsubscribeAsync(user.TelegramUserId);
        await _bot.SendMessage(chatId, removed ? UzMessages.UnsubscribeSuccess : UzMessages.NotSubscribed);
    }

    // ─────────────────────────────────────────────────────────────────────
    //  /admin
    // ─────────────────────────────────────────────────────────────────────

    private async Task HandleAdmin(long chatId, BotUser user)
    {
        if (!user.IsAdmin)
        {
            await _bot.SendMessage(chatId, UzMessages.NoPermission);
            return;
        }
        await _bot.SendMessage(chatId, UzMessages.AdminWelcome,
            parseMode: ParseMode.Markdown,
            replyMarkup: BotKeyboards.AdminPanel());
    }

    // ─────────────────────────────────────────────────────────────────────
    //  PUBLIC HELPERS — also called by CallbackQueryHandler
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>Fetches current weather, sends result, saves search history.</summary>
    public async Task SendWeatherAsync(long chatId, BotUser user, string city)
    {
        try
        {
            var weather = await _weatherService.GetCurrentWeatherAsync(city);
            if (weather == null)
            {
                await _bot.SendMessage(chatId, UzMessages.CityNotFound);
                return;
            }

            var text = string.Format(UzMessages.WeatherResult,
                weather.CityName, weather.Country,
                weather.Temp, weather.FeelsLike,
                weather.Humidity, weather.WindSpeed,
                weather.Description,
                weather.UpdatedAt.ToString("HH:mm"));

            await _bot.SendMessage(chatId, text,
                parseMode: ParseMode.Markdown,
                replyMarkup: BotKeyboards.WeatherResult(weather.CityName));

            await SaveSearchHistoryAsync(user, weather.CityName, weather.ShortSummary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending weather for city {City}", city);
            await _bot.SendMessage(chatId, UzMessages.WeatherError);
        }
    }

    /// <summary>Fetches 3-day forecast and sends formatted result.</summary>
    public async Task SendForecastAsync(long chatId, BotUser user, string city)
    {
        try
        {
            var result = await _weatherService.GetForecastAsync(city);
            if (result == null)
            {
                await _bot.SendMessage(chatId, UzMessages.CityNotFound);
                return;
            }

            var sb = new StringBuilder(string.Format(UzMessages.ForecastHeader, result.Value.CityName));

            foreach (var day in result.Value.Days)
            {
                var date = DateTimeOffset.FromUnixTimeSeconds(day.Dt).UtcDateTime;
                var dateLabel = date.ToString("dd.MM.yyyy, dddd", new CultureInfo("uz-UZ"));
                var desc = day.Weather.FirstOrDefault()?.Description ?? string.Empty;
                if (desc.Length > 0)
                    desc = char.ToUpperInvariant(desc[0]) + desc[1..];

                sb.Append(string.Format(UzMessages.ForecastDay,
                    dateLabel,
                    Math.Round(day.Main.TempMin, 0),
                    Math.Round(day.Main.TempMax, 0),
                    desc));
            }

            await _bot.SendMessage(chatId, sb.ToString(),
                parseMode: ParseMode.Markdown,
                replyMarkup: BotKeyboards.ForecastResult(result.Value.CityName));

            await SaveSearchHistoryAsync(user, result.Value.CityName, "3 kunlik bashorat");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending forecast for city {City}", city);
            await _bot.SendMessage(chatId, UzMessages.WeatherError);
        }
    }

    /// <summary>Sends the last-5-searches history list.</summary>
    public async Task SendHistoryAsync(long chatId, BotUser user)
    {
        var history = await _db.SearchHistories
            .Where(sh => sh.TelegramUserId == user.TelegramUserId)
            .OrderByDescending(sh => sh.SearchedAt)
            .Take(5)
            .ToListAsync();

        if (!history.Any())
        {
            await _bot.SendMessage(chatId, UzMessages.HistoryEmpty);
            return;
        }

        var sb = new StringBuilder(UzMessages.HistoryHeader);
        foreach (var item in history)
            sb.Append(string.Format(UzMessages.HistoryItem,
                item.CityName,
                item.WeatherSummary,
                item.SearchedAt.ToString("dd.MM.yyyy HH:mm")));

        await _bot.SendMessage(chatId, sb.ToString(), parseMode: ParseMode.Markdown);
    }

    /// <summary>Sends the user's personal statistics.</summary>
    public async Task SendMyStatsAsync(long chatId, BotUser user)
    {
        var totalSearches = await _db.SearchHistories
            .CountAsync(sh => sh.TelegramUserId == user.TelegramUserId);

        var subscription = await _subscriptionService.GetSubscriptionAsync(user.TelegramUserId);
        var subscribedText = subscription != null ? UzMessages.SubscribedYes : UzMessages.SubscribedNo;
        var cityText       = subscription?.City ?? UzMessages.NoCity;

        await _bot.SendMessage(chatId,
            string.Format(UzMessages.UserStats,
                user.FirstName, totalSearches,
                user.RegisteredAt.ToString("dd.MM.yyyy"),
                subscribedText, cityText),
            parseMode: ParseMode.Markdown);
    }

    // ─────────────────────────────────────────────────────────────────────
    //  Private helpers
    // ─────────────────────────────────────────────────────────────────────

    private async Task SaveSearchHistoryAsync(BotUser user, string city, string summary)
    {
        _db.SearchHistories.Add(new SearchHistory
        {
            TelegramUserId = user.TelegramUserId,
            CityName       = city,
            WeatherSummary = summary
        });
        await _db.SaveChangesAsync();
    }
}
