namespace SkyBot.Models;

public class District
{
    public int Id { get; set; }
    public int RegionId { get; set; }
    public long SoatoId { get; set; }
    public string NameUz { get; set; } = string.Empty;
    public string NameOz { get; set; } = string.Empty;
    public string NameRu { get; set; } = string.Empty;

    /// <summary>
    /// Returns a name suitable for weather search (removes " tumani", " shahri", etc.)
    /// </summary>
    public string GetSearchName()
    {
        var name = NameUz;
        var suffixes = new[] { " tumani", " shahri", " shaxar", " tumani", " tumani" };
        foreach (var suffix in suffixes)
        {
            if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                name = name[..^suffix.Length];
                break;
            }
        }
        return name;
    }
}
