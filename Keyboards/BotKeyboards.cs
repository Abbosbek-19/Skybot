using System.Collections.Generic;
using System.Linq;
using SkyBot.Localization;
using Telegram.Bot.Types.ReplyMarkups;

namespace SkyBot.Keyboards;

/// <summary>
/// Factory class for all InlineKeyboardMarkup objects used in the bot.
/// All button labels come from UzMessages constants — never hardcoded here.
/// CallbackData strings are short English keys parsed by CallbackQueryHandler.
/// </summary>
public static class BotKeyboards
{
    // ─── Main Menu ───────────────────────────────────────────────────────────
    // Shown after /start and when user presses "🏠 Asosiy menyu"

    public static InlineKeyboardMarkup MainMenu() => new(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData(UzMessages.BtnWeather,   "cmd_weather"),
            InlineKeyboardButton.WithCallbackData(UzMessages.BtnForecast,  "cmd_forecast"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData(UzMessages.BtnSubscribe,   "cmd_subscribe"),
            InlineKeyboardButton.WithCallbackData(UzMessages.BtnUnsubscribe, "cmd_unsubscribe"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData(UzMessages.BtnHistory, "cmd_history"),
            InlineKeyboardButton.WithCallbackData(UzMessages.BtnStats,   "cmd_stats"),
        },
    });

    // ─── Weather Result ──────────────────────────────────────────────────────
    // Shown below every weather result message.
    // {city} is embedded in callback data so the handler knows which city to use.

    public static InlineKeyboardMarkup WeatherResult(string city)
    {
        var encodedCity = Uri.EscapeDataString(city);
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(UzMessages.BtnRefresh,       $"refresh_{encodedCity}"),
                InlineKeyboardButton.WithCallbackData(UzMessages.BtnForecast,      $"forecast_{encodedCity}"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(UzMessages.BtnSubscribeCity, $"subscribe_city_{encodedCity}"),
                InlineKeyboardButton.WithCallbackData(UzMessages.BtnMainMenu,      "main_menu"),
            },
        });
    }

    // ─── Forecast result ─────────────────────────────────────────────────────

    public static InlineKeyboardMarkup ForecastResult(string city)
    {
        var encodedCity = Uri.EscapeDataString(city);
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(UzMessages.BtnWeather,  $"refresh_{encodedCity}"),
                InlineKeyboardButton.WithCallbackData(UzMessages.BtnMainMenu, "main_menu"),
            },
        });
    }

    // ─── Subscribe: hour selection ───────────────────────────────────────────
    // Shown after user types a city name for subscription.
    // CallbackData format: "hour_{cityEncoded}_{hour}"

    public static InlineKeyboardMarkup SubscribeHourPicker(string city)
    {
        var c = Uri.EscapeDataString(city);
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("06:00", $"hour_{c}_6"),
                InlineKeyboardButton.WithCallbackData("07:00", $"hour_{c}_7"),
                InlineKeyboardButton.WithCallbackData("08:00", $"hour_{c}_8"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("09:00", $"hour_{c}_9"),
                InlineKeyboardButton.WithCallbackData("12:00", $"hour_{c}_12"),
                InlineKeyboardButton.WithCallbackData("18:00", $"hour_{c}_18"),
            },
        });
    }

    // ─── Subscribe: confirm/cancel ───────────────────────────────────────────
    // CallbackData format: "confirm_sub_{cityEncoded}_{hour}" and "cancel_sub"

    public static InlineKeyboardMarkup SubscribeConfirm(string city, int hour)
    {
        var c = Uri.EscapeDataString(city);
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(UzMessages.BtnConfirmYes, $"confirm_sub_{c}_{hour}"),
                InlineKeyboardButton.WithCallbackData(UzMessages.BtnConfirmNo,  "cancel_sub"),
            },
        });
    }

    // ─── Admin Panel ─────────────────────────────────────────────────────────

    public static InlineKeyboardMarkup AdminPanel() => new(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData(UzMessages.BtnAdminUsers,     "admin_users_1"),
            InlineKeyboardButton.WithCallbackData(UzMessages.BtnAdminStats,     "admin_stats"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData(UzMessages.BtnAdminBroadcast, "admin_broadcast"),
            InlineKeyboardButton.WithCallbackData(UzMessages.BtnAdminBan,       "admin_ban"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData(UzMessages.BtnAdminMakeAdmin, "admin_makeadmin"),
        },
    });

    // ─── Admin: paginated user list ──────────────────────────────────────────

    public static InlineKeyboardMarkup AdminUserListPager(int currentPage, int totalPages)
    {
        var buttons = new List<InlineKeyboardButton>();

        if (currentPage > 1)
            buttons.Add(InlineKeyboardButton.WithCallbackData("◀️ Oldingi", $"admin_users_{currentPage - 1}"));

        if (currentPage < totalPages)
            buttons.Add(InlineKeyboardButton.WithCallbackData("Keyingi ▶️", $"admin_users_{currentPage + 1}"));

        var rows = new List<InlineKeyboardButton[]>();
        if (buttons.Any())
            rows.Add(buttons.ToArray());

        rows.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData("🔙 Admin menyu", "admin_panel")
        });

        return new InlineKeyboardMarkup(rows);
    }
}
