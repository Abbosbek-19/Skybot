namespace SkyBot.Localization;

/// <summary>
/// Central repository for ALL user-facing Uzbek strings.
/// NEVER hardcode Uzbek text elsewhere — always reference this class.
/// Use string.Format() or String.Format() with these constants where {0}, {1}, etc. appear.
/// </summary>
public static class UzMessages
{
    // ────────────────────────────────────────────────────────────────────────
    //  START & WELCOME
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>{0} = user's first name</summary>
    public const string Welcome =
        "👋 Assalomu alaykum, {0}! SkyBot'ga xush kelibsiz!\n" +
        "🌤 Men sizga istalgan shahar ob-havosini aytib beraman.\n" +
        "Quyidagi menyudan foydalaning:";

    /// <summary>{0} = user's first name</summary>
    public const string AlreadyRegistered =
        "👋 Qaytib keldingiz, {0}! Menyudan tanlang:";

    // ────────────────────────────────────────────────────────────────────────
    //  HELP
    // ────────────────────────────────────────────────────────────────────────

    public const string HelpText =
        "📖 *Mavjud buyruqlar:*\n\n" +
        "/start — Botni ishga tushirish\n" +
        "/help — Yordam\n" +
        "/weather — Hozirgi ob-havo\n" +
        "/forecast — 3 kunlik ob-havo\n" +
        "/subscribe — Kunlik obuna\n" +
        "/unsubscribe — Obunani bekor qilish\n" +
        "/history — So'nggi qidiruvlar\n" +
        "/mystats — Mening statistikam";

    // ────────────────────────────────────────────────────────────────────────
    //  WEATHER RESULTS
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// {0}=city, {1}=country, {2}=temp, {3}=feels_like,
    /// {4}=humidity, {5}=wind_speed, {6}=description, {7}=updated_time
    /// </summary>
    public const string WeatherResult =
        "🌤 *{0}, {1} ob-havosi*\n" +
        "🌡 Harorat: {2}°C (his qilinadi: {3}°C)\n" +
        "💧 Namlik: {4}%\n" +
        "💨 Shamol: {5} m/s\n" +
        "☁️ Holat: {6}\n" +
        "🕐 Yangilangan: {7} UTC";

    /// <summary>{0} = city name</summary>
    public const string ForecastHeader = "📅 *{0} shahri uchun 3 kunlik ob-havo:*\n\n";

    /// <summary>{0}=date, {1}=min_temp, {2}=max_temp, {3}=description</summary>
    public const string ForecastDay =
        "📆 *{0}*\n" +
        "🌡 Min: {1}°C | Maks: {2}°C\n" +
        "☁️ {3}\n\n";

    public const string CityNotFound =
        "❌ Shahar topilmadi. Iltimos, shahar nomini to'g'ri kiriting.";

    public const string WeatherError =
        "⚠️ Ob-havo xizmati hozir ishlamayapti. Iltimos, keyinroq urinib ko'ring.";

    public const string EnterCityName = "🏙 Shahar nomini kiriting:";

    // ────────────────────────────────────────────────────────────────────────
    //  SUBSCRIPTION
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>{0}=city, {1}=hour</summary>
    public const string SubscribeSuccess =
        "✅ Siz *{0}* shahri uchun kunlik ob-havoga muvaffaqiyatli obuna bo'ldingiz!\n" +
        "⏰ Har kuni soat *{1}:00 UTC* da yuboriladi.";

    /// <summary>{0}=city</summary>
    public const string AlreadySubscribed =
        "ℹ️ Siz allaqachon *{0}* shahriga obunasiz.\n" +
        "/unsubscribe buyrug'i bilan bekor qilishingiz mumkin.";

    public const string UnsubscribeSuccess =
        "🔕 Obunangiz muvaffaqiyatli bekor qilindi.";

    public const string NotSubscribed =
        "ℹ️ Siz hech qanday obunaga ega emassiz.";

    public const string SubscribeEnterCity =
        "🏙 Obuna bo'lmoqchi bo'lgan shahar nomini kiriting:";

    public const string SubscribeSelectHour =
        "⏰ Kunlik ob-havoni qaysi soatda olishni xohlaysiz? (UTC vaqt)";

    /// <summary>{0}=city, {1}=hour</summary>
    public const string SubscribeConfirm =
        "✅ *{0}* shahri uchun soat *{1}:00 UTC* da obuna qilishni tasdiqlaysizmi?";

    /// <summary>{0}=city, {1}=weather_text</summary>
    public const string DailyWeatherMessage =
        "🌅 Xayrli tong! *{0}* shahri uchun bugungi ob-havo:\n\n{1}";

    // ────────────────────────────────────────────────────────────────────────
    //  SEARCH HISTORY
    // ────────────────────────────────────────────────────────────────────────

    public const string HistoryHeader = "📜 *So'nggi 5 ta qidiruv:*\n\n";

    /// <summary>{0}=city, {1}=summary, {2}=date</summary>
    public const string HistoryItem = "🔹 {0} — {1} ({2})\n";

    public const string HistoryEmpty =
        "📭 Siz hali hech qanday shahar qidirmadingiz.";

    // ────────────────────────────────────────────────────────────────────────
    //  STATS
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// {0}=username, {1}=total_searches, {2}=registered_date,
    /// {3}=subscribed_yes_no, {4}=subscribed_city
    /// </summary>
    public const string UserStats =
        "📊 *Sizning statistikangiz:*\n\n" +
        "👤 Foydalanuvchi: {0}\n" +
        "🔍 Jami qidiruvlar: {1}\n" +
        "📅 Ro'yxatdan o'tgan: {2}\n" +
        "🔔 Obuna: {3}\n" +
        "🏙 Obuna shahri: {4}";

    public const string SubscribedYes = "Ha ✅";
    public const string SubscribedNo = "Yo'q ❌";
    public const string NoCity = "—";

    // ────────────────────────────────────────────────────────────────────────
    //  ERRORS & GENERAL
    // ────────────────────────────────────────────────────────────────────────

    public const string GeneralError =
        "⚠️ Xatolik yuz berdi. Iltimos, qaytadan urinib ko'ring.";

    public const string NoPermission =
        "⛔ Sizda bu buyruqni ishlatish huquqi yo'q.";

    public const string UnknownCommand =
        "❓ Noma'lum buyruq. Yordam uchun /help yozing.";

    public const string BotBanned =
        "🚫 Siz botdan bloklangansiz. Admin bilan bog'laning.";

    // ────────────────────────────────────────────────────────────────────────
    //  ADMIN PANEL
    // ────────────────────────────────────────────────────────────────────────

    public const string AdminWelcome =
        "🛠 *Admin paneli*\nQuyidagi amallardan birini tanlang:";

    /// <summary>{0}=total_users, {1}=today_searches, {2}=active_subscriptions</summary>
    public const string AdminStats =
        "📊 *Bot statistikasi:*\n\n" +
        "👥 Jami foydalanuvchilar: {0}\n" +
        "🔍 Bugungi qidiruvlar: {1}\n" +
        "🔔 Faol obunalar: {2}";

    /// <summary>{0}=page_number, {1}=user_list_text</summary>
    public const string AdminUserList =
        "👥 *Foydalanuvchilar ro'yxati ({0}-sahifa):*\n\n{1}";

    /// <summary>{0}=name, {1}=telegram_id, {2}=search_count, {3}=subscribed_yes_no</summary>
    public const string AdminUserItem =
        "👤 {0} | ID: {1} | Qidiruvlar: {2} | Obuna: {3}\n";

    public const string BroadcastPrompt =
        "📢 Barcha foydalanuvchilarga yuboriladigan xabarni kiriting:";

    /// <summary>{0}=count of recipients</summary>
    public const string BroadcastDone =
        "✅ Xabar {0} ta foydalanuvchiga yuborildi.";

    /// <summary>{0}=target user's name/id</summary>
    public const string MakeAdminSuccess =
        "✅ Foydalanuvchi {0} admin qilindi.";

    public const string MakeAdminNotFound =
        "❌ Bunday ID li foydalanuvchi topilmadi.";

    /// <summary>{0}=target username or id</summary>
    public const string BanSuccess =
        "🚫 Foydalanuvchi {0} bloklandi.";

    public const string BanNotFound =
        "❌ Bunday ID li foydalanuvchi topilmadi.";

    public const string EnterUserIdForAdmin =
        "🔢 Admin qilmoqchi bo'lgan foydalanuvchining Telegram ID sini kiriting:";

    public const string EnterUserIdForBan =
        "🔢 Bloklamoqchi bo'lgan foydalanuvchining Telegram ID sini kiriting:";

    // ────────────────────────────────────────────────────────────────────────
    //  BUTTON LABELS (used in BotKeyboards.cs)
    // ────────────────────────────────────────────────────────────────────────

    public const string BtnWeather        = "🌤 Ob-havoni ko'rish";
    public const string BtnForecast       = "📅 3 kunlik ob-havo";
    public const string BtnSubscribe      = "🔔 Obuna bo'lish";
    public const string BtnUnsubscribe    = "🔕 Obunani bekor qilish";
    public const string BtnHistory        = "📜 Qidiruvlarim";
    public const string BtnStats          = "📊 Statistikam";
    public const string BtnRefresh        = "🔄 Yangilash";
    public const string BtnSubscribeCity  = "🔔 Shu shaharga obuna";
    public const string BtnMainMenu       = "🏠 Asosiy menyu";
    public const string BtnConfirmYes     = "✅ Ha, obuna bo'laman";
    public const string BtnConfirmNo      = "❌ Bekor qilish";
    public const string BtnAdminUsers     = "👥 Foydalanuvchilar";
    public const string BtnAdminStats     = "📊 Bot statistikasi";
    public const string BtnAdminBroadcast = "📢 Xabar yuborish";
    public const string BtnAdminBan       = "🚫 Foydalanuvchini bloklash";
    public const string BtnAdminMakeAdmin = "⭐ Admin qilish";
}
