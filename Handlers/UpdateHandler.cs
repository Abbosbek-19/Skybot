using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
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
/// Entry point for every Telegram Update received at the webhook.
/// Responsibilities:
///   1. Register new users / update LastActiveAt
///   2. Block banned users
///   3. Route Message updates → CommandHandler or plain-text state handling
///   4. Route CallbackQuery updates → CallbackQueryHandler
/// </summary>
public class UpdateHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly AppDbContext _db;
    private readonly CommandHandler _commandHandler;
    private readonly CallbackQueryHandler _callbackQueryHandler;
    private readonly AdminHandler _adminHandler;
    private readonly UserStateService _stateService;
    private readonly LocationService _locationService;
    private readonly IConfiguration _config;
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(
        ITelegramBotClient bot,
        AppDbContext db,
        CommandHandler commandHandler,
        CallbackQueryHandler callbackQueryHandler,
        AdminHandler adminHandler,
        UserStateService stateService,
        LocationService locationService,
        IConfiguration config,
        ILogger<UpdateHandler> logger)
    {
        _bot = bot;
        _db = db;
        _commandHandler = commandHandler;
        _callbackQueryHandler = callbackQueryHandler;
        _adminHandler = adminHandler;
        _stateService = stateService;
        _locationService = locationService;
        _config = config;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(Update update)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message != null)
                        await HandleMessageAsync(update.Message);
                    break;

                case UpdateType.CallbackQuery:
                    if (update.CallbackQuery != null)
                        await HandleCallbackQueryAsync(update.CallbackQuery);
                    break;

                default:
                    // Ignore other update types (InlineQuery, ChosenInlineResult, etc.)
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in UpdateHandler");
        }
    }

    // ── Message routing ───────────────────────────────────────────────────────

    private async Task HandleMessageAsync(Message message)
    {
        // Ignore non-text messages (photos, stickers, etc.)
        if (message.Text == null || message.From == null) return;

        var telegramUser = message.From;
        var chatId = message.Chat.Id;

        // Get or create the BotUser record
        var user = await GetOrCreateUserAsync(telegramUser);

        // Block banned users silently (or with a message)
        if (user.IsBanned)
        {
            await _bot.SendMessage(chatId, MessageResolver.Get(user, "BotBanned"));
            return;
        }

        // Update last active timestamp
        user.LastActiveAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var text = message.Text.Trim();

        // ── Commands (start with '/') ─────────────────────────────────────────
        if (text.StartsWith('/'))
        {
            // Clear any pending state when user sends a new command
            _stateService.ClearState(user.TelegramUserId);
            await _commandHandler.HandleAsync(message, user);
            return;
        }

        // ── Plain text — check conversation state ─────────────────────────────
        var state = _stateService.GetState(user.TelegramUserId);

        if (state == null)
        {
            // No active state — unknown input
            await _bot.SendMessage(chatId, MessageResolver.Get(user, "UnknownCommand"));
            return;
        }

        switch (state)
        {
            case "awaiting_weather_city":
                _stateService.ClearState(user.TelegramUserId);
                await _commandHandler.SendWeatherAsync(chatId, user, text);
                break;

            case "awaiting_forecast_city":
                _stateService.ClearState(user.TelegramUserId);
                await _commandHandler.SendForecastAsync(chatId, user, text);
                break;

            case "awaiting_reminder_city":
                _stateService.ClearState(user.TelegramUserId);
                await _bot.SendMessage(chatId, MessageResolver.Get(user, "ReminderSelectHour"),
                    replyMarkup: BotKeyboards.ReminderHourPicker(user, text));
                break;

            case "awaiting_broadcast":
                await _adminHandler.HandleBroadcastTextAsync(chatId, user, text);
                break;

            case "awaiting_ban_id":
                await _adminHandler.HandleBanUserAsync(chatId, user, text);
                break;

            case "awaiting_makeadmin_id":
                await _adminHandler.HandleMakeAdminAsync(chatId, user, text);
                break;

            default:
                _stateService.ClearState(user.TelegramUserId);
                await _bot.SendMessage(chatId, MessageResolver.Get(user, "UnknownCommand"));
                break;
        }
    }

    // ── CallbackQuery routing ─────────────────────────────────────────────────

    private async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery)
    {
        if (callbackQuery.From == null || callbackQuery.Message == null) return;

        var user = await GetOrCreateUserAsync(callbackQuery.From);

        if (user.IsBanned)
        {
            await _bot.AnswerCallbackQuery(callbackQuery.Id, UzMessages.BotBanned);
            return;
        }

        user.LastActiveAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        await _callbackQueryHandler.HandleAsync(callbackQuery, user);
    }

    // ── User management ───────────────────────────────────────────────────────

    private async Task<BotUser> GetOrCreateUserAsync(User telegramUser)
    {
        var user = await _db.BotUsers
            .FirstOrDefaultAsync(u => u.TelegramUserId == telegramUser.Id);

        if (user != null)
            return user;

        // New user — check if this is the configured admin
        var adminId = _config.GetValue<long>("BotSettings:AdminTelegramId");

        user = new BotUser
        {
            TelegramUserId = telegramUser.Id,
            Username = telegramUser.Username,
            FirstName = telegramUser.FirstName,
            LastName = telegramUser.LastName,
            IsAdmin = telegramUser.Id == adminId && adminId != 0
        };

        _db.BotUsers.Add(user);
        await _db.SaveChangesAsync();

        _logger.LogInformation("New user registered: {UserId} (@{Username})",
            telegramUser.Id, telegramUser.Username);

        return user;
    }
}
