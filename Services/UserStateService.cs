using System.Collections.Concurrent;

namespace SkyBot.Services;

/// <summary>
/// Tracks per-user conversation state in memory using a thread-safe dictionary.
/// Used to know what to do with a plain text message from the user.
///
/// Example states:
///   "awaiting_weather_city"    — next message is a city name for /weather
///   "awaiting_forecast_city"   — next message is a city name for /forecast
///   "awaiting_subscribe_city"  — next message is a city name for /subscribe
///   "awaiting_broadcast"       — admin: next message is the broadcast text
///   "awaiting_ban_id"          — admin: next message is a user ID to ban
///   "awaiting_makeadmin_id"    — admin: next message is a user ID to make admin
///
/// State is cleared immediately after it's consumed.
/// Registered as Singleton so state persists for the app lifetime.
/// </summary>
public class UserStateService
{
    private readonly ConcurrentDictionary<long, string> _states = new();

    /// <summary>Sets the conversation state for a user.</summary>
    public void SetState(long telegramUserId, string state)
        => _states[telegramUserId] = state;

    /// <summary>Returns the current state, or null if none.</summary>
    public string? GetState(long telegramUserId)
        => _states.TryGetValue(telegramUserId, out var state) ? state : null;

    /// <summary>Clears the conversation state for a user.</summary>
    public void ClearState(long telegramUserId)
        => _states.TryRemove(telegramUserId, out _);

    /// <summary>Returns true if the user has an active conversation state.</summary>
    public bool HasState(long telegramUserId)
        => _states.ContainsKey(telegramUserId);
}
