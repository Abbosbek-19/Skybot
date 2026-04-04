using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkyBot.Data;
using SkyBot.Keyboards;
using SkyBot.Localization;
using SkyBot.Models;
using SkyBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace SkyBot.Handlers;

/// <summary>
/// Handles admin-only actions: stats, user list, broadcast, ban, make-admin.
/// All text is in Uzbek via UzMessages constants.
/// Called by CallbackQueryHandler and UpdateHandler (for plain-text admin states).
/// </summary>
public class AdminHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly AppDbContext _db;
    private readonly SubscriptionService _subscriptionService;
    private readonly UserStateService _stateService;
    private readonly ILogger<AdminHandler> _logger;

    private const int UsersPerPage = 10;

    public AdminHandler(
        ITelegramBotClient bot,
        AppDbContext db,
        SubscriptionService subscriptionService,
        UserStateService stateService,
        ILogger<AdminHandler> logger)
    {
        _bot = bot;
        _db = db;
        _subscriptionService = subscriptionService;
        _stateService = stateService;
        _logger = logger;
    }

    // ── Bot statistics ────────────────────────────────────────────────────────

    public async Task SendStatsAsync(long chatId, BotUser requestingUser)
    {
        if (!requestingUser.IsAdmin)
        {
            await _bot.SendMessage(chatId, UzMessages.NoPermission);
            return;
        }

        var totalUsers = await _db.BotUsers.CountAsync();
        var todayStart = DateTime.UtcNow.Date;
        var todaySearches = await _db.SearchHistories
            .CountAsync(sh => sh.SearchedAt >= todayStart);
        var activeSubscriptions = await _subscriptionService.GetTotalSubscriptionsCountAsync();

        await _bot.SendMessage(
            chatId,
            string.Format(UzMessages.AdminStats, totalUsers, todaySearches, activeSubscriptions),
            parseMode: ParseMode.Markdown);
    }

    // ── Paginated user list ───────────────────────────────────────────────────

    public async Task SendUserListAsync(long chatId, BotUser requestingUser, int page)
    {
        if (!requestingUser.IsAdmin)
        {
            await _bot.SendMessage(chatId, UzMessages.NoPermission);
            return;
        }

        var totalUsers = await _db.BotUsers.CountAsync();
        var totalPages = (int)Math.Ceiling(totalUsers / (double)UsersPerPage);
        page = Math.Max(1, Math.Min(page, totalPages));

        var users = await _db.BotUsers
            .OrderByDescending(u => u.RegisteredAt)
            .Skip((page - 1) * UsersPerPage)
            .Take(UsersPerPage)
            .ToListAsync();

        var sb = new System.Text.StringBuilder();
        foreach (var u in users)
        {
            var searchCount = await _db.SearchHistories.CountAsync(sh => sh.TelegramUserId == u.TelegramUserId);
            var subStatus = u.IsSubscribed ? UzMessages.SubscribedYes : UzMessages.SubscribedNo;
            sb.Append(string.Format(UzMessages.AdminUserItem,
                u.FirstName,
                u.TelegramUserId,
                searchCount,
                subStatus));
        }

        await _bot.SendMessage(
            chatId,
            string.Format(UzMessages.AdminUserList, page, sb.ToString()),
            parseMode: ParseMode.Markdown,
            replyMarkup: BotKeyboards.AdminUserListPager(page, totalPages));
    }

    // ── Broadcast ─────────────────────────────────────────────────────────────

    public async Task HandleBroadcastTextAsync(long chatId, BotUser admin, string broadcastText)
    {
        if (!admin.IsAdmin) { await _bot.SendMessage(chatId, UzMessages.NoPermission); return; }

        var users = await _db.BotUsers
            .Where(u => !u.IsBanned)
            .ToListAsync();

        int sent = 0;
        foreach (var user in users)
        {
            try
            {
                await _bot.SendMessage(user.TelegramUserId, broadcastText, parseMode: ParseMode.Markdown);
                sent++;
                await Task.Delay(35); // Telegram rate limit: ~30 messages/sec
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not broadcast to user {UserId}", user.TelegramUserId);
            }
        }

        _stateService.ClearState(admin.TelegramUserId);
        await _bot.SendMessage(chatId, string.Format(UzMessages.BroadcastDone, sent));
    }

    // ── Ban user ──────────────────────────────────────────────────────────────

    public async Task HandleBanUserAsync(long chatId, BotUser admin, string inputId)
    {
        if (!admin.IsAdmin) { await _bot.SendMessage(chatId, UzMessages.NoPermission); return; }

        _stateService.ClearState(admin.TelegramUserId);

        if (!long.TryParse(inputId.Trim(), out var targetId))
        {
            await _bot.SendMessage(chatId, UzMessages.BanNotFound);
            return;
        }

        var target = await _db.BotUsers.FirstOrDefaultAsync(u => u.TelegramUserId == targetId);
        if (target == null)
        {
            await _bot.SendMessage(chatId, UzMessages.BanNotFound);
            return;
        }

        target.IsBanned = true;
        await _db.SaveChangesAsync();

        await _bot.SendMessage(
            chatId,
            string.Format(UzMessages.BanSuccess, target.FirstName),
            parseMode: ParseMode.Markdown);
    }

    // ── Make admin ────────────────────────────────────────────────────────────

    public async Task HandleMakeAdminAsync(long chatId, BotUser admin, string inputId)
    {
        if (!admin.IsAdmin) { await _bot.SendMessage(chatId, UzMessages.NoPermission); return; }

        _stateService.ClearState(admin.TelegramUserId);

        if (!long.TryParse(inputId.Trim(), out var targetId))
        {
            await _bot.SendMessage(chatId, UzMessages.MakeAdminNotFound);
            return;
        }

        var target = await _db.BotUsers.FirstOrDefaultAsync(u => u.TelegramUserId == targetId);
        if (target == null)
        {
            await _bot.SendMessage(chatId, UzMessages.MakeAdminNotFound);
            return;
        }

        target.IsAdmin = true;
        await _db.SaveChangesAsync();

        await _bot.SendMessage(
            chatId,
            string.Format(UzMessages.MakeAdminSuccess, target.FirstName),
            parseMode: ParseMode.Markdown);
    }
}
