namespace Domain.Models;

public class CollectedVersion
{
    public string Version { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public string SourceUrl { get; set; } = string.Empty;
}