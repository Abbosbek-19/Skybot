namespace SkyBot.Localization;

/// <summary>
/// O'zbek tilidagi barcha xabarlar.
/// </summary>
public static class UzMessages
{
    // ─── START & WELCOME ─────────────────────────────────────────────────

    public const string Welcome =
        "👋 Assalomu alaykum, {0}! SkyBot'ga xush kelibsiz!\n" +
        "🌤 Men sizga istalgan shahar ob-havosini aytib beraman.\n" +
        "Quyidagi menyudan foydalaning:";

    public const string AlreadyRegistered =
        "👋 Qaytib keldingiz, {0}! Menyudan tanlang:";

    // ─── LANGUAGE SELECTION ──────────────────────────────────────────────

    public const string SelectLanguage =
        "🌐 Tilni tanlang / Выберите язык / Select language:";

    public const string LangUzbek = "🇺 O'zbekcha";
    public const string LangEnglish = "🇬 English";
    public const string LangRussian = "🇷🇺 Русский";

    public const string LanguageChanged =
        "✅ Til o'zgartirildi!";

    // ─── HELP ────────────────────────────────────────────────────────────

    public const string HelpText =
        "📖 *Mavjud buyruqlar:*\n\n" +
        "/start — Botni ishga tushirish\n" +
        "/help — Yordam\n" +
        "/weather — Hozirgi ob-havo\n" +
        "/forecast — 3 kunlik ob-havo\n" +
        "/reminder — Kunlik eslatma\n" +
        "/cancel_reminder — Eslatmani bekor qilish\n" +
        "/location — Joylashuvni tanlash\n" +
        "/history — So'nggi qidiruvlar\n" +
        "/mystats — Mening statistikam\n" +
        "/settings — Sozlamalar";

    // ─── WEATHER RESULTS ─────────────────────────────────────────────────

    public const string WeatherResult =
        "🌤 *{0}, {1} ob-havosi*\n" +
        "🌡 Harorat: {2}°C (his qilinadi: {3}°C)\n" +
        "💧 Namlik: {4}%\n" +
        "💨 Shamol: {5} m/s\n" +
        "☁️ Holat: {6}\n" +
        "🕐 Yangilangan: {7} UTC";

    public const string ForecastHeader = "📅 *{0} shahri uchun 3 kunlik ob-havo:*\n\n";
    public const string ForecastDay =
        "📆 *{0}*\n" +
        "🌡 Min: {1}°C | Maks: {2}°C\n" +
        "☁️ {3}\n\n";

    public const string CityNotFound =
        "❌ Shahar topilmadi. Iltimos, shahar nomini to'g'ri kiriting.";

    public const string WeatherError =
        "⚠️ Ob-havo xizmati hozir ishlamayapti. Iltimos, keyinroq urinib ko'ring.";

    public const string EnterCityName = "🏙 Shahar nomini kiriting:";

    // ─── LOCATION SELECTION ──────────────────────────────────────────────

    public const string LocationSelectRegion =
        "🗺 *Viloyatni tanlang:*";

    public const string LocationSelectDistrict =
        "🏘 *Tumanni tanlang:*";

    public const string LocationBackToRegions =
        "🔙 Viloyatlar ro'yxatiga";

    public const string LocationEnterRegion =
        "🗺 Viloyat nomini yozing:\n\n" +
        "Masalan: Andijon viloyati, Toshkent shahri, Samarqand viloyati";

    public const string LocationEnterCity =
        "🏙 Shahar yoki tuman nomini yozing:\n\n" +
        "Masalan: Andijon tumani, Chilonzor tumani";

    public const string LocationRegionNotFound =
        "❌ Viloyat topilmadi. Iltimos, viloyat nomini to'g'ri yozing.\n\n" +
        "Masalan: Andijon viloyati, Toshkent shahri";

    // ─── REMINDER (avvalgi obuna) ────────────────────────────────────────

    public const string ReminderSuccess =
        "✅ Siz *{0}* shahri uchun kunlik eslatmaga muvaffaqiyatli obuna bo'ldingiz!\n" +
        "⏰ Har kuni soat *{1}:00 UTC* da yuboriladi.";

    public const string AlreadyReminded =
        "ℹ️ Siz allaqachon *{0}* shahriga eslatma o'rnatgansiz.\n" +
        "/cancel_reminder buyrug'i bilan bekor qilishingiz mumkin.";

    public const string CancelReminderSuccess =
        "🔕 Eslatma muvaffaqiyatli bekor qilindi.";

    public const string NotReminded =
        "ℹ️ Sizda hech qanday eslatma o'rnatilmagan.";

    public const string ReminderEnterCity =
        "🏙 Eslatma o'rnatmoqchi bo'lgan shahar nomini kiriting:";

    public const string ReminderSelectHour =
        "⏰ Kunlik ob-havoni qaysi soatda olishni xohlaysiz? (UTC vaqt)";

    public const string ReminderConfirm =
        "✅ *{0}* shahri uchun soat *{1}:00 UTC* da eslatma o'rnatishni tasdiqlaysizmi?";

    public const string DailyReminderMessage =
        "🌅 Xayrli tong! *{0}* shahri uchun bugungi ob-havo:\n\n{1}";

    // ─── SEARCH HISTORY ──────────────────────────────────────────────────

    public const string HistoryHeader = "📜 *So'nggi 5 ta qidiruv:*\n\n";
    public const string HistoryItem = "🔹 {0} — {1} ({2})\n";
    public const string HistoryEmpty =
        "📭 Siz hali hech qanday shahar qidirmadingiz.";

    // ─── STATS ───────────────────────────────────────────────────────────

    public const string UserStats =
        "📊 *Sizning statistikangiz:*\n\n" +
        "👤 Foydalanuvchi: {0}\n" +
        "🔍 Jami qidiruvlar: {1}\n" +
        "📅 Ro'yxatdan o'tgan: {2}\n" +
        "🔔 Eslatma: {3}\n" +
        "🏙 Eslatma shahri: {4}";

    public const string SubscribedYes = "Ha ✅";
    public const string SubscribedNo = "Yo'q ❌";
    public const string NoCity = "—";

    // ─── SETTINGS ────────────────────────────────────────────────────────

    public const string SettingsTitle =
        "⚙️ *Sozlamalar*\n\n" +
        "🌐 Hozirgi til: {0}\n\n" +
        "Tilni o'zgartirish uchun quyidagi tugmalardan foydalaning:";

    public const string CurrentLangUz = "🇺🇿 O'zbekcha";
    public const string CurrentLangEn = "🇬🇧 English";
    public const string CurrentLangRu = "🇷🇺 Русский";

    // ─── ERRORS & GENERAL ────────────────────────────────────────────────

    public const string GeneralError =
        "⚠️ Xatolik yuz berdi. Iltimos, qaytadan urinib ko'ring.";

    public const string NoPermission =
        "⛔ Sizda bu buyruqni ishlatish huquqi yo'q.";

    public const string UnknownCommand =
        "❓ Noma'lum buyruq. Yordam uchun /help yozing.";

    public const string BotBanned =
        "🚫 Siz botdan bloklangansiz. Admin bilan bog'laning.";

    // ─── ADMIN PANEL ─────────────────────────────────────────────────────

    public const string AdminWelcome =
        "🛠 *Admin paneli*\nQuyidagi amallardan birini tanlang:";

    public const string AdminStats =
        "📊 *Bot statistikasi:*\n\n" +
        "👥 Jami foydalanuvchilar: {0}\n" +
        "🔍 Bugungi qidiruvlar: {1}\n" +
        "🔔 Faol eslatmalar: {2}";

    public const string AdminUserList =
        "👥 *Foydalanuvchilar ro'yxati ({0}-sahifa):*\n\n{1}";

    public const string AdminUserItem =
        "👤 {0} | ID: {1} | Qidiruvlar: {2} | Eslatma: {3}\n";

    public const string BroadcastPrompt =
        "📢 Barcha foydalanuvchilarga yuboriladigan xabarni kiriting:";

    public const string BroadcastDone =
        "✅ Xabar {0} ta foydalanuvchiga yuborildi.";

    public const string MakeAdminSuccess =
        "✅ Foydalanuvchi {0} admin qilindi.";

    public const string MakeAdminNotFound =
        "❌ Bunday ID li foydalanuvchi topilmadi.";

    public const string BanSuccess =
        "🚫 Foydalanuvchi {0} bloklandi.";

    public const string BanNotFound =
        "❌ Bunday ID li foydalanuvchi topilmadi.";

    public const string EnterUserIdForAdmin =
        "🔢 Admin qilmoqchi bo'lgan foydalanuvchining Telegram ID sini kiriting:";

    public const string EnterUserIdForBan =
        "🔢 Bloklamoqchi bo'lgan foydalanuvchining Telegram ID sini kiriting:";

    // ─── BUTTON LABELS ───────────────────────────────────────────────────

    public const string BtnWeather = "🌤 Ob-havoni ko'rish";
    public const string BtnForecast = "📅 3 kunlik ob-havo";
    public const string BtnReminder = "🔔 Eslatma";
    public const string BtnCancelReminder = "🔕 Eslatmani bekor qilish";
    public const string BtnHistory = "📜 Qidiruvlarim";
    public const string BtnStats = "📊 Statistikam";
    public const string BtnRefresh = "🔄 Yangilash";
    public const string BtnReminderCity = "🔔 Shu shaharga eslatma";
    public const string BtnMainMenu = "🏠 Asosiy menyu";
    public const string BtnConfirmYes = "✅ Ha, eslatma o'rnataman";
    public const string BtnConfirmNo = "❌ Bekor qilish";
    public const string BtnAdminUsers = "👥 Foydalanuvchilar";
    public const string BtnAdminStats = "📊 Bot statistikasi";
    public const string BtnAdminBroadcast = "📢 Xabar yuborish";
    public const string BtnAdminBan = "🚫 Foydalanuvchini bloklash";
    public const string BtnAdminMakeAdmin = "⭐ Admin qilish";
    public const string BtnSettings = "⚙️ Sozlamalar";
    public const string BtnChooseLocation = "🗍 Joylashuvni tanlash";
    public const string BtnNextPage = "Keyingi ▶️";
    public const string BtnPrevPage = "◀️ Oldingi";
    public const string BtnBack = "🔙 Orqaga";
    public const string BtnChangeLanguage = "🌐 Tilni o'zgartirish";
}
