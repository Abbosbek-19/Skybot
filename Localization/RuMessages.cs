namespace SkyBot.Localization;

/// <summary>
/// Russian messages.
/// </summary>
public static class RuMessages
{
    // ─── START & WELCOME ─────────────────────────────────────────────────

    public const string Welcome =
        "👋 Здравствуйте, {0}! Добро пожаловать в SkyBot!\n" +
        "🌤 Я могу сообщить погоду любого города.\n" +
        "Используйте меню ниже:";

    public const string AlreadyRegistered =
        "👋 С возвращением, {0}! Выберите из меню:";

    // ─── LANGUAGE SELECTION ──────────────────────────────────────────────

    public const string SelectLanguage =
        "🌐 Выберите язык / Tilni tanlang / Select language:";

    public const string LangUzbek = "🇺 O'zbekcha";
    public const string LangEnglish = "🇬 English";
    public const string LangRussian = "🇷🇺 Русский";

    public const string LanguageChanged =
        "✅ Язык изменён!";

    // ─── HELP ────────────────────────────────────────────────────────────

    public const string HelpText =
        "📖 *Доступные команды:*\n\n" +
        "/start — Запустить бота\n" +
        "/help — Помощь\n" +
        "/weather — Текущая погода\n" +
        "/forecast — Прогноз на 3 дня\n" +
        "/reminder — Ежедневное напоминание\n" +
        "/cancel_reminder — Отменить напоминание\n" +
        "/location — Выбрать местоположение\n" +
        "/history — Последние поиски\n" +
        "/mystats — Моя статистика\n" +
        "/settings — Настройки";

    // ─── WEATHER RESULTS ─────────────────────────────────────────────────

    public const string WeatherResult =
        "🌤 *Погода в {0}, {1}*\n" +
        "🌡 Температура: {2}°C (ощущается как: {3}°C)\n" +
        "💧 Влажность: {4}%\n" +
        "💨 Ветер: {5} м/с\n" +
        "☁️ Состояние: {6}\n" +
        "🕐 Обновлено: {7} UTC";

    public const string ForecastHeader = "📅 *Прогноз на 3 дня для {0}:*\n\n";
    public const string ForecastDay =
        "📆 *{0}*\n" +
        "🌡 Мин: {1}°C | Макс: {2}°C\n" +
        "☁️ {3}\n\n";

    public const string CityNotFound =
        "❌ Город не найден. Пожалуйста, введите правильное название города.";

    public const string WeatherError =
        "⚠️ Служба погоды временно недоступна. Попробуйте позже.";

    public const string EnterCityName = "🏙 Введите название города:";

    // ─── LOCATION SELECTION ──────────────────────────────────────────────

    public const string LocationSelectRegion =
        "🗺 *Выберите область:*";

    public const string LocationSelectDistrict =
        "🏘 *Выберите район:*";

    public const string LocationBackToRegions =
        "🔙 Назад к областям";

    public const string LocationEnterRegion =
        "🗺 Введите название области:\n\n" +
        "Пример: Андижанская область, город Ташкент";

    public const string LocationEnterCity =
        "🏙 Введите название города или района:\n\n" +
        "Пример: Андижанский район, Чиланзарский район";

    public const string LocationRegionNotFound =
        "❌ Область не найдена. Пожалуйста, введите правильное название.\n\n" +
        "Пример: Андижанская область, город Ташкент";

    // ─── REMINDER ────────────────────────────────────────────────────────

    public const string ReminderSuccess =
        "✅ Вы успешно подписались на ежедневные напоминания для *{0}*!\n" +
        "⏰ Будет отправляться каждый день в *{1}:00 UTC*.";

    public const string AlreadyReminded =
        "ℹ️ У вас уже установлено напоминание для *{0}*.\n" +
        "Используйте /cancel_reminder для отмены.";

    public const string CancelReminderSuccess =
        "🔕 Напоминание успешно отменено.";

    public const string NotReminded =
        "ℹ️ У вас нет установленных напоминаний.";

    public const string ReminderEnterCity =
        "🏙 Введите название города для напоминания:";

    public const string ReminderSelectHour =
        "⏰ В какое время вы хотите получать ежедневную погоду? (время UTC)";

    public const string ReminderConfirm =
        "✅ Подтвердить установку напоминания для *{0}* в *{1}:00 UTC*?";

    public const string DailyReminderMessage =
        "🌅 Доброе утро! Сегодняшняя погода для *{0}*:\n\n{1}";

    // ─── SEARCH HISTORY ──────────────────────────────────────────────────

    public const string HistoryHeader = "📜 *Последние 5 поисков:*\n\n";
    public const string HistoryItem = "🔹 {0} — {1} ({2})\n";
    public const string HistoryEmpty =
        "📭 Вы ещё не искали ни один город.";

    // ─── STATS ──────────────────────────────────────────────────────────

    public const string UserStats =
        "📊 *Ваша статистика:*\n\n" +
        "👤 Пользователь: {0}\n" +
        "🔍 Всего поисков: {1}\n" +
        "📅 Зарегистрирован: {2}\n" +
        "🔔 Напоминание: {3}\n" +
        "🏙 Город напоминания: {4}";

    public const string SubscribedYes = "Да ✅";
    public const string SubscribedNo = "Нет ❌";
    public const string NoCity = "—";

    // ─── SETTINGS ────────────────────────────────────────────────────────

    public const string SettingsTitle =
        "⚙️ *Настройки*\n\n" +
        "🌐 Текущий язык: {0}\n\n" +
        "Используйте кнопки ниже для смены языка:";

    public const string CurrentLangUz = "🇺 O'zbekcha";
    public const string CurrentLangEn = "🇬🇧 English";
    public const string CurrentLangRu = "🇷🇺 Русский";

    // ─── ERRORS & GENERAL ────────────────────────────────────────────────

    public const string GeneralError =
        "⚠️ Произошла ошибка. Попробуйте снова.";

    public const string NoPermission =
        "⛔ У вас нет прав для использования этой команды.";

    public const string UnknownCommand =
        "❓ Неизвестная команда. Введите /help для помощи.";

    public const string BotBanned =
        "🚫 Вы заблокированы в этом боте. Свяжитесь с администратором.";

    // ─── ADMIN PANEL ─────────────────────────────────────────────────────

    public const string AdminWelcome =
        "🛠 *Панель администратора*\nВыберите действие:";

    public const string AdminStats =
        "📊 *Статистика бота:*\n\n" +
        "👥 Всего пользователей: {0}\n" +
        "🔍 Поисков сегодня: {1}\n" +
        "🔔 Активных напоминаний: {2}";

    public const string AdminUserList =
        "👥 *Список пользователей (страница {0}):*\n\n{1}";

    public const string AdminUserItem =
        "👤 {0} | ID: {1} | Поисков: {2} | Напоминание: {3}\n";

    public const string BroadcastPrompt =
        "📢 Введите сообщение для рассылки всем пользователям:";

    public const string BroadcastDone =
        "✅ Сообщение отправлено {0} пользователям.";

    public const string MakeAdminSuccess =
        "✅ Пользователь {0} назначен администратором.";

    public const string MakeAdminNotFound =
        "❌ Пользователь с таким ID не найден.";

    public const string BanSuccess =
        "🚫 Пользователь {0} заблокирован.";

    public const string BanNotFound =
        "❌ Пользователь с таким ID не найден.";

    public const string EnterUserIdForAdmin =
        "🔢 Введите Telegram ID пользователя для назначения администратором:";

    public const string EnterUserIdForBan =
        "🔢 Введите Telegram ID пользователя для блокировки:";

    // ─── BUTTON LABELS ───────────────────────────────────────────────────

    public const string BtnWeather = "🌤 Посмотреть погоду";
    public const string BtnForecast = "📅 Прогноз на 3 дня";
    public const string BtnReminder = "🔔 Напоминание";
    public const string BtnCancelReminder = "🔕 Отменить напоминание";
    public const string BtnHistory = "📜 Мои поиски";
    public const string BtnStats = "📊 Моя статистика";
    public const string BtnRefresh = "🔄 Обновить";
    public const string BtnReminderCity = "🔔 Напоминание для этого города";
    public const string BtnMainMenu = "🏠 Главное меню";
    public const string BtnConfirmYes = "✅ Да, установить напоминание";
    public const string BtnConfirmNo = "❌ Отмена";
    public const string BtnAdminUsers = "👥 Пользователи";
    public const string BtnAdminStats = "📊 Статистика бота";
    public const string BtnAdminBroadcast = "📢 Рассылка";
    public const string BtnAdminBan = "🚫 Заблокировать";
    public const string BtnAdminMakeAdmin = "⭐ Назначить админом";
    public const string BtnSettings = "⚙️ Настройки";
    public const string BtnChooseLocation = "🗍 Выбрать местоположение";
    public const string BtnNextPage = "Далее ▶️";
    public const string BtnPrevPage = "◀️ Назад";
    public const string BtnBack = "🔙 Назад";
    public const string BtnChangeLanguage = "🌐 Сменить язык";
}
