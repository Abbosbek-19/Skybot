namespace SkyBot.Models;

public class Village
{
    public int Id { get; set; }
    public int DistrictId { get; set; }
    public long SoatoId { get; set; }
    public string NameUz { get; set; } = string.Empty;
    public string NameOz { get; set; } = string.Empty;
    public string NameRu { get; set; } = string.Empty;

    /// <summary>
    /// Returns a name suitable for weather search (removes " qf", " MFY", etc.)
    /// </summary>
    public string GetSearchName()
    {
        var name = NameUz;
        var suffixes = new[] { " qf", " MFY", " mahallasi", " posyolka", " qishlog'i" };
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
