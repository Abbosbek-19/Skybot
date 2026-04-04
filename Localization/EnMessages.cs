namespace SkyBot.Localization;

/// <summary>
/// English messages.
/// </summary>
public static class EnMessages
{
    // ─── START & WELCOME ─────────────────────────────────────────────────

    public const string Welcome =
        "👋 Hello, {0}! Welcome to SkyBot!\n" +
        "🌤 I can tell you the weather of any city.\n" +
        "Use the menu below:";

    public const string AlreadyRegistered =
        "👋 Welcome back, {0}! Choose from the menu:";

    // ─── LANGUAGE SELECTION ──────────────────────────────────────────────

    public const string SelectLanguage =
        "🌐 Select language / Tilni tanlang / Выберите язык:";

    public const string LangUzbek = "🇺 O'zbekcha";
    public const string LangEnglish = "🇬 English";
    public const string LangRussian = "🇷🇺 Русский";

    public const string LanguageChanged =
        "✅ Language changed!";

    // ─── HELP ────────────────────────────────────────────────────────────

    public const string HelpText =
        "📖 *Available commands:*\n\n" +
        "/start — Start the bot\n" +
        "/help — Help\n" +
        "/weather — Current weather\n" +
        "/forecast — 3-day forecast\n" +
        "/reminder — Daily reminder\n" +
        "/cancel_reminder — Cancel reminder\n" +
        "/location — Select location\n" +
        "/history — Recent searches\n" +
        "/mystats — My statistics\n" +
        "/settings — Settings";

    // ─── WEATHER RESULTS ─────────────────────────────────────────────────

    public const string WeatherResult =
        "🌤 *Weather in {0}, {1}*\n" +
        "🌡 Temperature: {2}°C (feels like: {3}°C)\n" +
        "💧 Humidity: {4}%\n" +
        "💨 Wind: {5} m/s\n" +
        "☁️ Condition: {6}\n" +
        "🕐 Updated: {7} UTC";

    public const string ForecastHeader = "📅 *3-day forecast for {0}:*\n\n";
    public const string ForecastDay =
        "📆 *{0}*\n" +
        "🌡 Min: {1}°C | Max: {2}°C\n" +
        "☁️ {3}\n\n";

    public const string CityNotFound =
        "❌ City not found. Please enter a valid city name.";

    public const string WeatherError =
        "⚠️ Weather service is currently unavailable. Please try again later.";

    public const string EnterCityName = "🏙 Enter city name:";

    // ─── LOCATION SELECTION ──────────────────────────────────────────────

    public const string LocationSelectRegion =
        "🗺 *Select a region:*";

    public const string LocationSelectDistrict =
        "🏘 *Select a district:*";

    public const string LocationBackToRegions =
        "🔙 Back to regions";

    public const string LocationEnterRegion =
        "🗺 Enter region name:\n\n" +
        "Example: Andijon viloyati, Toshkent shahri";

    public const string LocationEnterCity =
        "🏙 Enter city or district name:\n\n" +
        "Example: Andijon tumani, Chilonzor tumani";

    public const string LocationRegionNotFound =
        "❌ Region not found. Please enter a valid region name.\n\n" +
        "Example: Andijon viloyati, Toshkent shahri";

    // ─── REMINDER ────────────────────────────────────────────────────────

    public const string ReminderSuccess =
        "✅ You have successfully subscribed to daily reminders for *{0}*!\n" +
        "⏰ It will be sent every day at *{1}:00 UTC*.";

    public const string AlreadyReminded =
        "ℹ️ You already have a reminder set for *{0}*.\n" +
        "Use /cancel_reminder to cancel it.";

    public const string CancelReminderSuccess =
        "🔕 Reminder successfully cancelled.";

    public const string NotReminded =
        "ℹ️ You don't have any reminder set.";

    public const string ReminderEnterCity =
        "🏙 Enter the city name for the reminder:";

    public const string ReminderSelectHour =
        "⏰ What time would you like to receive the daily weather? (UTC time)";

    public const string ReminderConfirm =
        "✅ Confirm setting a reminder for *{0}* at *{1}:00 UTC*?";

    public const string DailyReminderMessage =
        "🌅 Good morning! Today's weather for *{0}*:\n\n{1}";

    // ─── SEARCH HISTORY ──────────────────────────────────────────────────

    public const string HistoryHeader = "📜 *Last 5 searches:*\n\n";
    public const string HistoryItem = "🔹 {0} — {1} ({2})\n";
    public const string HistoryEmpty =
        "📭 You haven't searched for any city yet.";

    // ─── STATS ──────────────────────────────────────────────────────────

    public const string UserStats =
        "📊 *Your statistics:*\n\n" +
        "👤 User: {0}\n" +
        "🔍 Total searches: {1}\n" +
        "📅 Registered: {2}\n" +
        "🔔 Reminder: {3}\n" +
        "🏙 Reminder city: {4}";

    public const string SubscribedYes = "Yes ✅";
    public const string SubscribedNo = "No ❌";
    public const string NoCity = "—";

    // ─── SETTINGS ────────────────────────────────────────────────────────

    public const string SettingsTitle =
        "⚙️ *Settings*\n\n" +
        "🌐 Current language: {0}\n\n" +
        "Use the buttons below to change language:";

    public const string CurrentLangUz = "🇺🇿 O'zbekcha";
    public const string CurrentLangEn = "🇬🇧 English";
    public const string CurrentLangRu = "🇷🇺 Русский";

    // ─── ERRORS & GENERAL ────────────────────────────────────────────────

    public const string GeneralError =
        "⚠️ An error occurred. Please try again.";

    public const string NoPermission =
        "⛔ You don't have permission to use this command.";

    public const string UnknownCommand =
        "❓ Unknown command. Type /help for assistance.";

    public const string BotBanned =
        "🚫 You are blocked from this bot. Contact the admin.";

    // ─── ADMIN PANEL ─────────────────────────────────────────────────────

    public const string AdminWelcome =
        "🛠 *Admin Panel*\nChoose an action:";

    public const string AdminStats =
        "📊 *Bot Statistics:*\n\n" +
        "👥 Total users: {0}\n" +
        "🔍 Today's searches: {1}\n" +
        "🔔 Active reminders: {2}";

    public const string AdminUserList =
        "👥 *User list (page {0}):*\n\n{1}";

    public const string AdminUserItem =
        "👤 {0} | ID: {1} | Searches: {2} | Reminder: {3}\n";

    public const string BroadcastPrompt =
        "📢 Enter the message to broadcast to all users:";

    public const string BroadcastDone =
        "✅ Message sent to {0} users.";

    public const string MakeAdminSuccess =
        "✅ User {0} has been made admin.";

    public const string MakeAdminNotFound =
        "❌ User with this ID not found.";

    public const string BanSuccess =
        "🚫 User {0} has been banned.";

    public const string BanNotFound =
        "❌ User with this ID not found.";

    public const string EnterUserIdForAdmin =
        "🔢 Enter the Telegram ID of the user to make admin:";

    public const string EnterUserIdForBan =
        "🔢 Enter the Telegram ID of the user to ban:";

    // ─── BUTTON LABELS ───────────────────────────────────────────────────

    public const string BtnWeather = "🌤 View Weather";
    public const string BtnForecast = "📅 3-Day Forecast";
    public const string BtnReminder = "🔔 Reminder";
    public const string BtnCancelReminder = "🔕 Cancel Reminder";
    public const string BtnHistory = "📜 My Searches";
    public const string BtnStats = "📊 My Stats";
    public const string BtnRefresh = "🔄 Refresh";
    public const string BtnReminderCity = "🔔 Set Reminder for this City";
    public const string BtnMainMenu = "🏠 Main Menu";
    public const string BtnConfirmYes = "✅ Yes, set reminder";
    public const string BtnConfirmNo = "❌ Cancel";
    public const string BtnAdminUsers = "👥 Users";
    public const string BtnAdminStats = "📊 Bot Stats";
    public const string BtnAdminBroadcast = "📢 Broadcast";
    public const string BtnAdminBan = "🚫 Ban User";
    public const string BtnAdminMakeAdmin = "⭐ Make Admin";
    public const string BtnSettings = "⚙️ Settings";
    public const string BtnChooseLocation = "🗍 Select Location";
    public const string BtnNextPage = "Next ▶️";
    public const string BtnPrevPage = "◀️ Prev";
    public const string BtnBack = "🔙 Back";
    public const string BtnChangeLanguage = "🌐 Change Language";
}
