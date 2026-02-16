using System.Text.Json;
using Domain.Interfaces;
using Domain.Models;
using Collectors.GitHub;

namespace Collectors.Collectors;

public class NginxGitHubCollector : IVersionCollector
{
    private readonly HttpClient _httpClient;
    public string ProductName => "Nginx";

    public NginxGitHubCollector(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.github.com/");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "VersionCollector/1.0");
    }

    public async Task<IReadOnlyCollection<CollectedVersion>> CollectAsync()
    {
        var response = await _httpClient.GetAsync("repos/nginx/nginx/releases");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var releases = JsonSerializer.Deserialize<List<GitHubRelease>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return releases?
            .Where(r => !r.Prerelease)
            .Select(r => new CollectedVersion
            {
                Version = r.TagName.TrimStart('v'),
                ReleaseDate = r.PublishedAt ?? r.CreatedAt,
                SourceUrl = r.HtmlUrl
            })
            .Take(30)
            .ToList()
            ?? new List<CollectedVersion>();
    }
}