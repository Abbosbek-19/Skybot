using SkyBot.Data;
using SkyBot.Keyboards;
using SkyBot.Localization;
using SkyBot.Models;
using SkyBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SkyBot.Handlers;

public class CallbackQueryHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly CommandHandler _commandHandler;
    private readonly SubscriptionService _subscriptionService;
    private readonly AdminHandler _adminHandler;
    private readonly UserStateService _stateService;
    private readonly LocationService _locationService;
    private readonly ILogger<CallbackQueryHandler> _logger;

    public CallbackQueryHandler(
        ITelegramBotClient bot,
        CommandHandler commandHandler,
        SubscriptionService subscriptionService,
        AdminHandler adminHandler,
        UserStateService stateService,
        LocationService locationService,
        ILogger<CallbackQueryHandler> logger)
    {
        _bot = bot;
        _commandHandler = commandHandler;
        _subscriptionService = subscriptionService;
        _adminHandler = adminHandler;
        _stateService = stateService;
        _locationService = locationService;
        _logger = logger;
    }

    public async Task HandleAsync(CallbackQuery callbackQuery, BotUser user)
    {
        var data = callbackQuery.Data ?? string.Empty;
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;

        await _bot.AnswerCallbackQuery(callbackQuery.Id);

        try
        {
            await DispatchAsync(data, chatId, messageId, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling callback: {Data}", data);
            await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "GeneralError"));
        }
    }

    private async Task DispatchAsync(string data, long chatId, int messageId, BotUser user)
    {
        // ── Main menu shortcuts ─────────────────────────────────────────

        switch (data)
        {
            case "cmd_weather":
                _stateService.SetState(user.TelegramUserId, "awaiting_weather_city");
                await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "EnterCityName"));
                return;

            case "cmd_forecast":
                _stateService.SetState(user.TelegramUserId, "awaiting_forecast_city");
                await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "EnterCityName"));
                return;

            case "cmd_reminder":
                _stateService.SetState(user.TelegramUserId, "awaiting_reminder_city");
                await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "ReminderEnterCity"));
                return;

            case "cmd_cancel_reminder":
            {
                var removed = await _subscriptionService.UnsubscribeAsync(user.TelegramUserId);
                await _bot.EditMessageText(chatId, messageId,
                    removed ? MessageResolver.Get(user, "CancelReminderSuccess") : MessageResolver.Get(user, "NotReminded"));
                return;
            }

            case "cmd_history":
                await _commandHandler.SendHistoryAsync(chatId, user);
                return;

            case "cmd_stats":
                await _commandHandler.SendMyStatsAsync(chatId, user);
                return;

            case "cmd_settings":
            {
                var currentLang = user.Language switch
                {
                    "en" => MessageResolver.Get(user, "CurrentLangEn"),
                    "ru" => MessageResolver.Get(user, "CurrentLangRu"),
                    _ => MessageResolver.Get(user, "CurrentLangUz")
                };
                var text = string.Format(MessageResolver.Get(user, "SettingsTitle"), currentLang);
                await _bot.EditMessageText(chatId, messageId, text,
                    replyMarkup: BotKeyboards.SettingsPanel(user));
                return;
            }

            case "cmd_change_language":
                await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "SelectLanguage"),
                    replyMarkup: BotKeyboards.LanguageChangePicker());
                return;

            case "main_menu":
                await _bot.EditMessageText(chatId, messageId,
                    string.Format(MessageResolver.Get(user, "AlreadyRegistered"), user.FirstName),
                    replyMarkup: BotKeyboards.MainMenu(user));
                return;

            case "cancel_reminder_flow":
                _stateService.ClearState(user.TelegramUserId);
                await _bot.EditMessageText(chatId, messageId,
                    string.Format(MessageResolver.Get(user, "AlreadyRegistered"), user.FirstName),
                    replyMarkup: BotKeyboards.MainMenu(user));
                return;

            case "admin_panel":
                if (!user.IsAdmin) { await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "NoPermission")); return; }
                await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "AdminWelcome"),
                    replyMarkup: BotKeyboards.AdminPanel(user));
                return;

            case "admin_stats":
                await _adminHandler.SendStatsAsync(chatId, user);
                return;

            case "admin_broadcast":
                if (!user.IsAdmin) { await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "NoPermission")); return; }
                _stateService.SetState(user.TelegramUserId, "awaiting_broadcast");
                await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "BroadcastPrompt"));
                return;

            case "admin_ban":
                if (!user.IsAdmin) { await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "NoPermission")); return; }
                _stateService.SetState(user.TelegramUserId, "awaiting_ban_id");
                await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "EnterUserIdForBan"));
                return;

            case "admin_makeadmin":
                if (!user.IsAdmin) { await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "NoPermission")); return; }
                _stateService.SetState(user.TelegramUserId, "awaiting_makeadmin_id");
                await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "EnterUserIdForAdmin"));
                return;

            // ── Language selection ────────────────────────────────────────

            case "lang_uz":
            case "lang_en":
            case "lang_ru":
            {
                var lang = data["lang_".Length..];
                user.Language = lang;
                await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "LanguageChanged"),
                    replyMarkup: BotKeyboards.MainMenu(user));
                return;
            }

            case "set_lang_uz":
            case "set_lang_en":
            case "set_lang_ru":
            {
                var lang = data["set_lang_".Length..];
                user.Language = lang;
                await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "LanguageChanged"),
                    replyMarkup: BotKeyboards.SettingsPanel(user));
                return;
            }

            // ── Location selection ────────────────────────────────────────

            case "cmd_choose_location":
                _stateService.SetState(user.TelegramUserId, "awaiting_weather_city");
                await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "EnterCityName"));
                return;
        }

        // ── Pattern-matched callbacks ─────────────────────────────────────

        if (data.StartsWith("refresh_", StringComparison.Ordinal))
        {
            var city = Uri.UnescapeDataString(data["refresh_".Length..]);
            await _commandHandler.SendWeatherAsync(chatId, user, city);
            return;
        }

        if (data.StartsWith("forecast_", StringComparison.Ordinal))
        {
            var city = Uri.UnescapeDataString(data["forecast_".Length..]);
            await _commandHandler.SendForecastAsync(chatId, user, city);
            return;
        }

        // "reminder_city_{encodedCity}" → start reminder flow
        if (data.StartsWith("reminder_city_", StringComparison.Ordinal))
        {
            var city = Uri.UnescapeDataString(data["reminder_city_".Length..]);
            var existing = await _subscriptionService.GetSubscriptionAsync(user.TelegramUserId);
            if (existing != null)
            {
                await _bot.EditMessageText(chatId, messageId,
                    string.Format(MessageResolver.Get(user, "AlreadyReminded"), existing.City));
                return;
            }
            await _bot.EditMessageText(chatId, messageId, MessageResolver.Get(user, "ReminderSelectHour"),
                replyMarkup: BotKeyboards.ReminderHourPicker(user, city));
            return;
        }

        // "hour_{encodedCity}_{hour}" → show confirm dialog
        if (data.StartsWith("hour_", StringComparison.Ordinal))
        {
            var payload = data["hour_".Length..];
            var lastUnderscore = payload.LastIndexOf('_');
            var city = Uri.UnescapeDataString(payload[..lastUnderscore]);
            var hour = int.Parse(payload[(lastUnderscore + 1)..]);

            await _bot.EditMessageText(chatId, messageId,
                string.Format(MessageResolver.Get(user, "ReminderConfirm"), city, hour),
                replyMarkup: BotKeyboards.ReminderConfirm(user, city, hour));
            return;
        }

        // "confirm_reminder_{encodedCity}_{hour}" → finalize reminder
        if (data.StartsWith("confirm_reminder_", StringComparison.Ordinal))
        {
            var payload = data["confirm_reminder_".Length..];
            var lastUnderscore = payload.LastIndexOf('_');
            var city = Uri.UnescapeDataString(payload[..lastUnderscore]);
            var hour = int.Parse(payload[(lastUnderscore + 1)..]); // UTC hour

            await _subscriptionService.SubscribeAsync(user.TelegramUserId, city, hour);

            // Convert UTC hour to Tashkent time (UTC+5) for display
            var tashkentHour = (hour + 5) % 24;

            await _bot.EditMessageText(chatId, messageId,
                string.Format(MessageResolver.Get(user, "ReminderSuccess"), city, tashkentHour));
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
