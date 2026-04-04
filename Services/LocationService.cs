using System.Text.Json;
using SkyBot.Models;

namespace SkyBot.Services;

public class LocationService
{
    private readonly List<Region> _regions = new();
    private readonly List<District> _districts = new();
    private readonly ILogger<LocationService> _logger;

    public LocationService(ILogger<LocationService> logger)
    {
        _logger = logger;
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            var regionsPath = Path.Combine(AppContext.BaseDirectory, "Data", "regions.json");
            if (File.Exists(regionsPath))
            {
                var json = File.ReadAllText(regionsPath);
                _regions.AddRange(JsonSerializer.Deserialize<List<Region>>(json, options) ?? new());
            }

            var districtsPath = Path.Combine(AppContext.BaseDirectory, "Data", "districts.json");
            if (File.Exists(districtsPath))
            {
                var json = File.ReadAllText(districtsPath);
                _districts.AddRange(JsonSerializer.Deserialize<List<District>>(json, options) ?? new());
            }
            
            _logger.LogInformation("✅ Loaded {RegionCount} regions and {DistrictCount} districts.", _regions.Count, _districts.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error loading regions/districts data.");
        }
    }

    public IEnumerable<Region> GetRegions() => _regions.OrderBy(r => r.NameUz);

    public IEnumerable<District> GetDistricts(int regionId) => 
        _districts.Where(d => d.RegionId == regionId).OrderBy(d => d.NameUz);

    public Region? GetRegion(int id) => _regions.FirstOrDefault(r => r.Id == id);
    
    public District? GetDistrict(int id) => _districts.FirstOrDefault(d => d.Id == id);
}
