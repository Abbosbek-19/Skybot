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

public class CommandHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly AppDbContext _db;
    private readonly IWeatherService _weatherService;
    private readonly SubscriptionService _subscriptionService;
    private readonly UserStateService _stateService;
    private readonly LocationService _locationService;
    private readonly ILogger<CommandHandler> _logger;

    public CommandHandler(
        ITelegramBotClient bot,
        AppDbContext db,
        IWeatherService weatherService,
        SubscriptionService subscriptionService,
        UserStateService stateService,
        LocationService locationService,
        ILogger<CommandHandler> logger)
    {
        _bot = bot;
        _db = db;
        _weatherService = weatherService;
        _subscriptionService = subscriptionService;
        _stateService = stateService;
        _locationService = locationService;
        _logger = logger;
    }

    public async Task HandleAsync(Message message, BotUser user)
    {
        var fullText = message.Text ?? string.Empty;
        var command = fullText.Split(' ')[0].Split('@')[0].ToLowerInvariant();
        var args = fullText.Contains(' ')
            ? fullText[(fullText.IndexOf(' ') + 1)..].Trim()
            : string.Empty;

        switch (command)
        {
            case "/start":       await HandleStart(message, user);             break;
            case "/help":        await HandleHelp(message.Chat.Id, user);      break;
            case "/weather":     await HandleWeather(message.Chat.Id, user, args); break;
            case "/forecast":    await HandleForecast(message.Chat.Id, user, args); break;
            case "/reminder":    await HandleReminder(message.Chat.Id, user, args); break;
            case "/cancel_reminder": await HandleCancelReminder(message.Chat.Id, user); break;
            case "/history":     await SendHistoryAsync(message.Chat.Id, user);   break;
            case "/mystats":     await SendMyStatsAsync(message.Chat.Id, user);   break;
            case "/location":    await HandleLocation(message.Chat.Id, user, args); break;
            case "/settings":    await HandleSettings(message.Chat.Id, user);     break;
            case "/admin":       await HandleAdmin(message.Chat.Id, user);        break;
            default:
                await _bot.SendMessage(message.Chat.Id, MessageResolver.Get(user, "UnknownCommand"));
                break;
        }
    }

    // ─── /start ────────────────────────────────────────────────────────────

    private async Task HandleStart(Message message, BotUser user)
    {
        var isNew = (DateTime.UtcNow - user.RegisteredAt).TotalSeconds < 5;
        
        if (isNew)
        {
            // Yangi foydalanuvchi — til tanlash
            await _bot.SendMessage(message.Chat.Id, MessageResolver.Get(user, "SelectLanguage"),
                replyMarkup: BotKeyboards.LanguagePicker());
        }
        else
        {
            // Eski foydalanuvchi — asosiy menyu
            var text = string.Format(MessageResolver.Get(user, "AlreadyRegistered"), user.FirstName);
            await _bot.SendMessage(message.Chat.Id, text,
                replyMarkup: BotKeyboards.MainMenu(user));
        }
    }

    // ─── /help ─────────────────────────────────────────────────────────────

    private async Task HandleHelp(long chatId, BotUser user)
    {
        await _bot.SendMessage(chatId, MessageResolver.Get(user, "HelpText"));
    }

    // ─── /weather ──────────────────────────────────────────────────────────

    private async Task HandleWeather(long chatId, BotUser user, string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            _stateService.SetState(user.TelegramUserId, "awaiting_weather_city");
            await _bot.SendMessage(chatId, MessageResolver.Get(user, "EnterCityName"));
            return;
        }
        await SendWeatherAsync(chatId, user, city);
    }

    // ─── /forecast ─────────────────────────────────────────────────────────

    private async Task HandleForecast(long chatId, BotUser user, string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            _stateService.SetState(user.TelegramUserId, "awaiting_forecast_city");
            await _bot.SendMessage(chatId, MessageResolver.Get(user, "EnterCityName"));
            return;
        }
        await SendForecastAsync(chatId, user, city);
    }

    // ─── /reminder ─────────────────────────────────────────────────────────

    private async Task HandleReminder(long chatId, BotUser user, string city)
    {
        var existing = await _subscriptionService.GetSubscriptionAsync(user.TelegramUserId);
        if (existing != null && string.IsNullOrWhiteSpace(city))
        {
            await _bot.SendMessage(chatId,
                string.Format(MessageResolver.Get(user, "AlreadyReminded"), existing.City));
            return;
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            _stateService.SetState(user.TelegramUserId, "awaiting_reminder_city");
            await _bot.SendMessage(chatId, MessageResolver.Get(user, "ReminderEnterCity"));
            return;
        }

        await _bot.SendMessage(chatId, MessageResolver.Get(user, "ReminderSelectHour"),
            replyMarkup: BotKeyboards.ReminderHourPicker(user, city));
    }

    // ─── /cancel_reminder ──────────────────────────────────────────────────

    private async Task HandleCancelReminder(long chatId, BotUser user)
    {
        var removed = await _subscriptionService.UnsubscribeAsync(user.TelegramUserId);
        await _bot.SendMessage(chatId, removed ? MessageResolver.Get(user, "CancelReminderSuccess") : MessageResolver.Get(user, "NotReminded"));
    }

    // ─── /admin ────────────────────────────────────────────────────────────

    private async Task HandleAdmin(long chatId, BotUser user)
    {
        if (!user.IsAdmin)
        {
            await _bot.SendMessage(chatId, MessageResolver.Get(user, "NoPermission"));
            return;
        }
        await _bot.SendMessage(chatId, MessageResolver.Get(user, "AdminWelcome"),
            replyMarkup: BotKeyboards.AdminPanel(user));
    }

    // ─── /location ─────────────────────────────────────────────────────────

    private async Task HandleLocation(long chatId, BotUser user, string context)
    {
        var validContexts = new[] { "weather", "forecast", "reminder" };
        var selectedContext = validContexts.Contains(context) ? context : "weather";

        _stateService.SetState(user.TelegramUserId, $"awaiting_{selectedContext}_city");
        await _bot.SendMessage(chatId, MessageResolver.Get(user, "EnterCityName"));
    }

    // ─── /settings ─────────────────────────────────────────────────────────

    private async Task HandleSettings(long chatId, BotUser user)
    {
        var currentLang = user.Language switch
        {
            "en" => MessageResolver.Get(user, "CurrentLangEn"),
            "ru" => MessageResolver.Get(user, "CurrentLangRu"),
            _ => MessageResolver.Get(user, "CurrentLangUz")
        };

        var text = string.Format(MessageResolver.Get(user, "SettingsTitle"), currentLang);
        await _bot.SendMessage(chatId, text,
            replyMarkup: BotKeyboards.SettingsPanel(user));
    }

    // ─── PUBLIC HELPERS ────────────────────────────────────────────────────

    public async Task SendWeatherAsync(long chatId, BotUser user, string city)
    {
        try
        {
            var weather = await _weatherService.GetCurrentWeatherAsync(city);
            if (weather == null)
            {
                await _bot.SendMessage(chatId, MessageResolver.Get(user, "CityNotFound"));
                return;
            }

            var text = string.Format(MessageResolver.Get(user, "WeatherResult"),
                weather.CityName, weather.Country,
                weather.Temp, weather.FeelsLike,
                weather.Humidity, weather.WindSpeed,
                weather.Description,
                weather.UpdatedAt.ToString("HH:mm"));

            await _bot.SendMessage(chatId, text,
                replyMarkup: BotKeyboards.WeatherResult(user, weather.CityName));

            await SaveSearchHistoryAsync(user, weather.CityName, weather.ShortSummary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending weather for city {City}", city);
            await _bot.SendMessage(chatId, MessageResolver.Get(user, "WeatherError"));
        }
    }

    public async Task SendForecastAsync(long chatId, BotUser user, string city)
    {
        try
        {
            var result = await _weatherService.GetForecastAsync(city);
            if (result == null)
            {
                await _bot.SendMessage(chatId, MessageResolver.Get(user, "CityNotFound"));
                return;
            }

            var sb = new StringBuilder(string.Format(MessageResolver.Get(user, "ForecastHeader"), result.Value.CityName));

            foreach (var day in result.Value.Days)
            {
                var date = DateTimeOffset.FromUnixTimeSeconds(day.Dt).UtcDateTime;
                var dateLabel = date.ToString("dd.MM.yyyy, dddd", new CultureInfo("uz-UZ"));
                var desc = WeatherService.TranslateCondition(day.Weather.FirstOrDefault()?.Description ?? string.Empty);
                if (desc.Length > 0)
                    desc = char.ToUpperInvariant(desc[0]) + desc[1..];

                sb.Append(string.Format(MessageResolver.Get(user, "ForecastDay"),
                    dateLabel,
                    Math.Round(day.Main.TempMin, 0),
                    Math.Round(day.Main.TempMax, 0),
                    desc));
            }

            await _bot.SendMessage(chatId, sb.ToString(),
                replyMarkup: BotKeyboards.ForecastResult(user, result.Value.CityName));

            await SaveSearchHistoryAsync(user, result.Value.CityName, "3-day forecast");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending forecast for city {City}", city);
            await _bot.SendMessage(chatId, MessageResolver.Get(user, "WeatherError"));
        }
    }

    public async Task SendHistoryAsync(long chatId, BotUser user)
    {
        var history = await _db.SearchHistories
            .Where(sh => sh.TelegramUserId == user.TelegramUserId)
            .OrderByDescending(sh => sh.SearchedAt)
            .Take(5)
            .ToListAsync();

        if (!history.Any())
        {
            await _bot.SendMessage(chatId, MessageResolver.Get(user, "HistoryEmpty"));
            return;
        }

        var sb = new StringBuilder(MessageResolver.Get(user, "HistoryHeader"));
        foreach (var item in history)
            sb.Append(string.Format(MessageResolver.Get(user, "HistoryItem"),
                item.CityName,
                item.WeatherSummary,
                item.SearchedAt.ToString("dd.MM.yyyy HH:mm")));

        await _bot.SendMessage(chatId, sb.ToString());
    }

    public async Task SendMyStatsAsync(long chatId, BotUser user)
    {
        var totalSearches = await _db.SearchHistories
            .CountAsync(sh => sh.TelegramUserId == user.TelegramUserId);

        var subscription = await _subscriptionService.GetSubscriptionAsync(user.TelegramUserId);
        var subscribedText = subscription != null ? MessageResolver.Get(user, "SubscribedYes") : MessageResolver.Get(user, "SubscribedNo");
        var cityText = subscription?.City ?? MessageResolver.Get(user, "NoCity");

        await _bot.SendMessage(chatId,
            string.Format(MessageResolver.Get(user, "UserStats"),
                user.FirstName, totalSearches,
                user.RegisteredAt.ToString("dd.MM.yyyy"),
                subscribedText, cityText));
    }

    // ─── Private helpers ───────────────────────────────────────────────────

    private async Task SaveSearchHistoryAsync(BotUser user, string city, string summary)
    {
        _db.SearchHistories.Add(new SearchHistory
        {
            TelegramUserId = user.TelegramUserId,
            CityName = city,
            WeatherSummary = summary
        });
        await _db.SaveChangesAsync();
    }
}
