using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SkyBot.Handlers;
using Telegram.Bot.Types;

namespace SkyBot.Controllers;

/// <summary>
/// Receives all incoming Telegram updates via webhook POST request.
/// The URL format is: POST /bot/{secretToken}
/// The secretToken is validated by Telegram automatically — Telegram will
/// only send requests to this endpoint if the token matches what we set
/// during SetWebhookAsync(), giving us a layer of security.
///
/// IMPORTANT: Always return 200 OK immediately, even on errors.
/// If we return non-200, Telegram will keep retrying the update.
/// </summary>
[ApiController]
[Route("bot")]
public class BotController : ControllerBase
{
    private readonly UpdateHandler _updateHandler;
    private readonly IConfiguration _config;
    private readonly ILogger<BotController> _logger;

    public BotController(
        UpdateHandler updateHandler,
        IConfiguration config,
        ILogger<BotController> logger)
    {
        _updateHandler = updateHandler;
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Telegram calls this endpoint for every update (message, callback query, etc.).
    /// The {secretToken} path segment must match BotSettings:SecretToken in appsettings.json.
    /// </summary>
    [HttpPost("{secretToken}")]
    public async Task<IActionResult> Post(
        [FromRoute] string secretToken,
        [FromBody] Update update)
    {
        // Validate the secret token to reject requests not coming from Telegram
        var expectedToken = _config["BotSettings:SecretToken"];
        if (secretToken != expectedToken)
        {
            _logger.LogWarning("Received request with invalid secret token: {Token}", secretToken);
            return Forbid();
        }

        // Process the update in the background — do NOT await a long operation here,
        // but for simplicity we await it and always return 200 regardless of outcome.
        try
        {
            await _updateHandler.HandleUpdateAsync(update);
        }
        catch (Exception ex)
        {
            // Log but never let exceptions break the 200 OK contract with Telegram
            _logger.LogError(ex, "Critical error processing update {UpdateId}", update.Id);
        }

        return Ok();
    }
}
