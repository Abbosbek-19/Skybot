using SkyBot.Data;
using SkyBot.Keyboards;
using SkyBot.Localization;
using SkyBot.Models;
using SkyBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SkyBot.Handlers;

/// <summary>
/// Handles all InlineKeyboard button presses (CallbackQuery updates).
/// Uses CommandHandler's public methods directly — no fake Message objects.
/// </summary>
public class CallbackQueryHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly CommandHandler _commandHandler;
    private readonly SubscriptionService _subscriptionService;
    private readonly AdminHandler _adminHandler;
    private readonly UserStateService _stateService;
    private readonly ILogger<CallbackQueryHandler> _logger;

    public CallbackQueryHandler(
        ITelegramBotClient bot,
        CommandHandler commandHandler,
        SubscriptionService subscriptionService,
        AdminHandler adminHandler,
        UserStateService stateService,
        ILogger<CallbackQueryHandler> logger)
    {
        _bot = bot;
        _commandHandler = commandHandler;
        _subscriptionService = subscriptionService;
        _adminHandler = adminHandler;
        _stateService = stateService;
        _logger = logger;
    }

    public async Task HandleAsync(CallbackQuery callbackQuery, BotUser user)
    {
        var data   = callbackQuery.Data ?? string.Empty;
        var chatId = callbackQuery.Message!.Chat.Id;

        // Always acknowledge the button tap first (removes the spinner)
        await _bot.AnswerCallbackQuery(callbackQuery.Id);

        try
        {
            await DispatchAsync(data, chatId, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling callback: {Data}", data);
            await _bot.SendMessage(chatId, UzMessages.GeneralError);
        }
    }

    private async Task DispatchAsync(string data, long chatId, BotUser user)
    {
        // ── Main menu button shortcuts ────────────────────────────────────
        switch (data)
        {
            case "cmd_weather":
                _stateService.SetState(user.TelegramUserId, "awaiting_weather_city");
                await _bot.SendMessage(chatId, UzMessages.EnterCityName);
                return;

            case "cmd_forecast":
                _stateService.SetState(user.TelegramUserId, "awaiting_forecast_city");
                await _bot.SendMessage(chatId, UzMessages.EnterCityName);
                return;

            case "cmd_subscribe":
                _stateService.SetState(user.TelegramUserId, "awaiting_subscribe_city");
                await _bot.SendMessage(chatId, UzMessages.SubscribeEnterCity);
                return;

            case "cmd_unsubscribe":
            {
                var removed = await _subscriptionService.UnsubscribeAsync(user.TelegramUserId);
                await _bot.SendMessage(chatId,
                    removed ? UzMessages.UnsubscribeSuccess : UzMessages.NotSubscribed);
                return;
            }

            case "cmd_history":
                await _commandHandler.SendHistoryAsync(chatId, user);
                return;

            case "cmd_stats":
                await _commandHandler.SendMyStatsAsync(chatId, user);
                return;

            case "main_menu":
                await _bot.SendMessage(chatId,
                    string.Format(UzMessages.AlreadyRegistered, user.FirstName),
                    parseMode: ParseMode.Markdown,
                    replyMarkup: BotKeyboards.MainMenu());
                return;

            case "cancel_sub":
                _stateService.ClearState(user.TelegramUserId);
                await _bot.SendMessage(chatId,
                    string.Format(UzMessages.AlreadyRegistered, user.FirstName),
                    parseMode: ParseMode.Markdown,
                    replyMarkup: BotKeyboards.MainMenu());
                return;

            case "admin_panel":
                if (!user.IsAdmin) { await _bot.SendMessage(chatId, UzMessages.NoPermission); return; }
                await _bot.SendMessage(chatId, UzMessages.AdminWelcome,
                    parseMode: ParseMode.Markdown,
                    replyMarkup: BotKeyboards.AdminPanel());
                return;

            case "admin_stats":
                await _adminHandler.SendStatsAsync(chatId, user);
                return;

            case "admin_broadcast":
                if (!user.IsAdmin) { await _bot.SendMessage(chatId, UzMessages.NoPermission); return; }
                _stateService.SetState(user.TelegramUserId, "awaiting_broadcast");
                await _bot.SendMessage(chatId, UzMessages.BroadcastPrompt);
                return;

            case "admin_ban":
                if (!user.IsAdmin) { await _bot.SendMessage(chatId, UzMessages.NoPermission); return; }
                _stateService.SetState(user.TelegramUserId, "awaiting_ban_id");
                await _bot.SendMessage(chatId, UzMessages.EnterUserIdForBan);
                return;

            case "admin_makeadmin":
                if (!user.IsAdmin) { await _bot.SendMessage(chatId, UzMessages.NoPermission); return; }
                _stateService.SetState(user.TelegramUserId, "awaiting_makeadmin_id");
                await _bot.SendMessage(chatId, UzMessages.EnterUserIdForAdmin);
                return;
        }

        // ── Pattern-matched callbacks ─────────────────────────────────────

        // "refresh_{encodedCity}" → refresh weather
        if (data.StartsWith("refresh_", StringComparison.Ordinal))
        {
            var city = Uri.UnescapeDataString(data["refresh_".Length..]);
            await _commandHandler.SendWeatherAsync(chatId, user, city);
            return;
        }

        // "forecast_{encodedCity}" → show forecast
        if (data.StartsWith("forecast_", StringComparison.Ordinal))
        {
            var city = Uri.UnescapeDataString(data["forecast_".Length..]);
            await _commandHandler.SendForecastAsync(chatId, user, city);
            return;
        }

        // "subscribe_city_{encodedCity}" → start subscription flow from weather result
        if (data.StartsWith("subscribe_city_", StringComparison.Ordinal))
        {
            var city = Uri.UnescapeDataString(data["subscribe_city_".Length..]);
            var existing = await _subscriptionService.GetSubscriptionAsync(user.TelegramUserId);
            if (existing != null)
            {
                await _bot.SendMessage(chatId,
                    string.Format(UzMessages.AlreadySubscribed, existing.City),
                    parseMode: ParseMode.Markdown);
                return;
            }
            await _bot.SendMessage(chatId, UzMessages.SubscribeSelectHour,
                replyMarkup: BotKeyboards.SubscribeHourPicker(city));
            return;
        }

        // "hour_{encodedCity}_{hour}" → show confirm dialog
        // Example: "hour_Toshkent_8"
        if (data.StartsWith("hour_", StringComparison.Ordinal))
        {
            // Split from the right so city names with underscores still work
            var payload = data["hour_".Length..];                  // "encodedCity_8"
            var lastUnderscore = payload.LastIndexOf('_');
            var city = Uri.UnescapeDataString(payload[..lastUnderscore]);
            var hour = int.Parse(payload[(lastUnderscore + 1)..]);

            await _bot.SendMessage(chatId,
                string.Format(UzMessages.SubscribeConfirm, city, hour),
                parseMode: ParseMode.Markdown,
                replyMarkup: BotKeyboards.SubscribeConfirm(city, hour));
            return;
        }

        // "confirm_sub_{encodedCity}_{hour}" → finalize subscription
        if (data.StartsWith("confirm_sub_", StringComparison.Ordinal))
        {
            var payload = data["confirm_sub_".Length..];
            var lastUnderscore = payload.LastIndexOf('_');
            var city = Uri.UnescapeDataString(payload[..lastUnderscore]);
            var hour = int.Parse(payload[(lastUnderscore + 1)..]);

            await _subscriptionService.SubscribeAsync(user.TelegramUserId, city, hour);
            await _bot.SendMessage(chatId,
                string.Format(UzMessages.SubscribeSuccess, city, hour),
                parseMode: ParseMode.Markdown);
            return;
        }

        // "admin_users_{page}" → paginated user list
        if (data.StartsWith("admin_users_", StringComparison.Ordinal))
        {
            var page = int.Parse(data["admin_users_".Length..]);
            await _adminHandler.SendUserListAsync(chatId, user, page);
            return;
        }

        _logger.LogWarning("Unhandled callback data: {Data}", data);
    }
}
