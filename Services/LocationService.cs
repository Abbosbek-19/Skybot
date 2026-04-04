using Microsoft.Extensions.Logging;

namespace SkyBot.Services;

/// <summary>
/// Service for location-related operations.
/// Currently simplified as region/district selection is removed.
/// </summary>
public class LocationService
{
    private readonly ILogger<LocationService> _logger;

    public LocationService(ILogger<LocationService> logger)
    {
        _logger = logger;
        _logger.LogInformation("LocationService initialized (simplified mode).");
    }
}
