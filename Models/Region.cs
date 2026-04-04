namespace SkyBot.Models;

public class Region
{
    public int Id { get; set; }
    public long SoatoId { get; set; }
    public string NameUz { get; set; } = string.Empty;
    public string NameOz { get; set; } = string.Empty;
    public string NameRu { get; set; } = string.Empty;
}
