namespace Collectors.GitHub;

public class GitHubRelease
{
    public string TagName { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string HtmlUrl { get; set; } = string.Empty;
    public bool Prerelease { get; set; }
}