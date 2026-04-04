using SkyBot.Localization;
using SkyBot.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace SkyBot.Keyboards;

/// <summary>
/// Factory class for all InlineKeyboardMarkup objects.
/// All button labels come from MessageResolver — supports 3 languages.
/// </summary>
public static class BotKeyboards
{
    // ─── Language Selection ────────────────────────────────────────────────

    public static InlineKeyboardMarkup LanguagePicker() => new(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData(UzMessages.LangUzbek, "lang_uz"),
            InlineKeyboardButton.WithCallbackData(UzMessages.LangEnglish, "lang_en"),
            InlineKeyboardButton.WithCallbackData(UzMessages.LangRussian, "lang_ru"),
        },
    });

    // ─── Main Menu ─────────────────────────────────────────────────────────

    public static InlineKeyboardMarkup MainMenu(BotUser user) => new(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnWeather"),   "cmd_weather"),
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnForecast"),  "cmd_forecast"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnReminder"),    "cmd_reminder"),
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnCancelReminder"), "cmd_cancel_reminder"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnHistory"), "cmd_history"),
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnStats"),   "cmd_stats"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnSettings"), "cmd_settings"),
        },
    });

    // ─── Weather Result ────────────────────────────────────────────────────

    public static InlineKeyboardMarkup WeatherResult(BotUser user, string city)
    {
        var encodedCity = Uri.EscapeDataString(city);
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Msg(user, "BtnRefresh"),       $"refresh_{encodedCity}"),
                InlineKeyboardButton.WithCallbackData(Msg(user, "BtnForecast"),      $"forecast_{encodedCity}"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Msg(user, "BtnReminderCity"),  $"reminder_city_{encodedCity}"),
                InlineKeyboardButton.WithCallbackData(Msg(user, "BtnMainMenu"),      "main_menu"),
            },
        });
    }

    // ─── Forecast result ───────────────────────────────────────────────────

    public static InlineKeyboardMarkup ForecastResult(BotUser user, string city)
    {
        var encodedCity = Uri.EscapeDataString(city);
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Msg(user, "BtnWeather"),  $"refresh_{encodedCity}"),
                InlineKeyboardButton.WithCallbackData(Msg(user, "BtnMainMenu"), "main_menu"),
            },
        });
    }

    // ─── Reminder: hour selection ──────────────────────────────────────────
    // Labels show Uzbekistan time (UTC+5), callback data sends UTC hour.
    // 06:00 UZT = 01:00 UTC, etc.

    public static InlineKeyboardMarkup ReminderHourPicker(BotUser user, string city)
    {
        var c = Uri.EscapeDataString(city);
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("06:00", $"hour_{c}_1"),
                InlineKeyboardButton.WithCallbackData("07:00", $"hour_{c}_2"),
                InlineKeyboardButton.WithCallbackData("08:00", $"hour_{c}_3"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("09:00", $"hour_{c}_4"),
                InlineKeyboardButton.WithCallbackData("11:00", $"hour_{c}_6"),
                InlineKeyboardButton.WithCallbackData("12:00", $"hour_{c}_7"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("13:00", $"hour_{c}_8"),
                InlineKeyboardButton.WithCallbackData("14:00", $"hour_{c}_9"),
                InlineKeyboardButton.WithCallbackData("17:00", $"hour_{c}_12"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("23:00", $"hour_{c}_18"),
            },
        });
    }

    // ─── Reminder: confirm/cancel ──────────────────────────────────────────

    public static InlineKeyboardMarkup ReminderConfirm(BotUser user, string city, int hour)
    {
        var c = Uri.EscapeDataString(city);
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Msg(user, "BtnConfirmYes"), $"confirm_reminder_{c}_{hour}"),
                InlineKeyboardButton.WithCallbackData(Msg(user, "BtnConfirmNo"),  "cancel_reminder_flow"),
            },
        });
    }

    // ─── Settings ───────────────────────────────────────────────────────────

    public static InlineKeyboardMarkup SettingsPanel(BotUser user) => new(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnChangeLanguage"), "cmd_change_language"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnMainMenu"), "main_menu"),
        },
    });

    // ─── Language Change ────────────────────────────────────────────────────

    public static InlineKeyboardMarkup LanguageChangePicker() => new(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData(UzMessages.LangUzbek, "set_lang_uz"),
            InlineKeyboardButton.WithCallbackData(UzMessages.LangEnglish, "set_lang_en"),
            InlineKeyboardButton.WithCallbackData(UzMessages.LangRussian, "set_lang_ru"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData(Msg(new BotUser { Language = "uz" }, "BtnBack"), "cmd_settings"),
        },
    });

    // ─── Admin Panel ───────────────────────────────────────────────────────

    public static InlineKeyboardMarkup AdminPanel(BotUser user) => new(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnAdminUsers"),     "admin_users_1"),
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnAdminStats"),     "admin_stats"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnAdminBroadcast"), "admin_broadcast"),
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnAdminBan"),       "admin_ban"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnAdminMakeAdmin"), "admin_makeadmin"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData(Msg(user, "BtnMainMenu"), "main_menu"),
        },
    });

    // ─── Admin: paginated user list ────────────────────────────────────────

    public static InlineKeyboardMarkup AdminUserListPager(BotUser user, int currentPage, int totalPages)
    {
        var buttons = new List<InlineKeyboardButton>();

        if (currentPage > 1)
            buttons.Add(InlineKeyboardButton.WithCallbackData(Msg(user, "BtnPrevPage"), $"admin_users_{currentPage - 1}"));

        if (currentPage < totalPages)
            buttons.Add(InlineKeyboardButton.WithCallbackData(Msg(user, "BtnNextPage"), $"admin_users_{currentPage + 1}"));

        var rows = new List<InlineKeyboardButton[]>();
        if (buttons.Any())
            rows.Add(buttons.ToArray());

        rows.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData("🔙 " + Msg(user, "BtnBack"), "admin_panel")
        });

        return new InlineKeyboardMarkup(rows);
    }

    // ─── Helper ─────────────────────────────────────────────────────────────

    private static string Msg(BotUser user, string key) => MessageResolver.Get(user, key);
}
