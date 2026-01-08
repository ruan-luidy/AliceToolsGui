using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AliceToolsGui.Wpf.Services;

public class GithubUpdateService : IGithubUpdateService
{
  private readonly HttpClient _httpClient;
  private const string GithubApiUrl = "https://api.github.com/repos/YOURREPO/YOURAPP/releases/latest";
  private const string CurrentVersion = "1.0.0";

  public GithubUpdateService()
  {
    _httpClient = new HttpClient();
    _httpClient.DefaultRequestHeaders.Add("User-Agent", "AliceToolsGui");
  }

  public async Task<bool> CheckForUpdatesAsync()
  {
    try
    {
      var response = await _httpClient.GetFromJsonAsync<GitHubRelease>(GithubApiUrl);

      if (response == null)
        return false;

      var latestVersion = response.TagName.TrimStart('v');
      var currentVersion = CurrentVersion;

      return CompareVersions(latestVersion, currentVersion) > 0;
    }
    catch
    {
      return false;
    }
  }

  public async Task<string> GetLatestVersionAsync()
  {
    try
    {
      var response = await _httpClient.GetFromJsonAsync<GitHubRelease>(GithubApiUrl);
      return response?.TagName.TrimStart('v') ?? CurrentVersion;
    }
    catch
    {
      return CurrentVersion;
    }
  }

  public async Task DownloadAndInstallUpdateAsync(IProgress<int>? progress = null)
  {
    var response = await _httpClient.GetFromJsonAsync<GitHubRelease>(GithubApiUrl);

    if (response?.Assets == null || response.Assets.Length == 0)
    {
      throw new Exception("Nenhum arquivo de atualização encontrado.");
    }

    var asset = response.Assets.FirstOrDefault(a => a.Name.EndsWith(".exe"));
    if (asset == null)
    {
      throw new Exception("Instalador não encontrado.");
    }

    var downloadPath = Path.Combine(Path.GetTempPath(), asset.Name);

    using var downloadResponse = await _httpClient.GetAsync(asset.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead);
    downloadResponse.EnsureSuccessStatusCode();

    var totalBytes = downloadResponse.Content.Headers.ContentLength ?? 0;
    var downloadedBytes = 0L;

    using var contentStream = await downloadResponse.Content.ReadAsStreamAsync();
    using var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

    var buffer = new byte[8192];
    int bytesRead;

    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
    {
      await fileStream.WriteAsync(buffer, 0, bytesRead);
      downloadedBytes += bytesRead;

      if (totalBytes > 0)
      {
        var percentComplete = (int)((downloadedBytes * 100) / totalBytes);
        progress?.Report(percentComplete);
      }
    }

    // Executar instalador
    Process.Start(new ProcessStartInfo
    {
      FileName = downloadPath,
      UseShellExecute = true
    });

    // Fechar aplicativo
    Environment.Exit(0);
  }

  private int CompareVersions(string version1, string version2)
  {
    var v1Parts = version1.Split('.');
    var v2Parts = version2.Split('.');

    for (int i = 0; i < Math.Max(v1Parts.Length, v2Parts.Length); i++)
    {
      var v1Part = i < v1Parts.Length ? int.Parse(v1Parts[i]) : 0;
      var v2Part = i < v2Parts.Length ? int.Parse(v2Parts[i]) : 0;

      if (v1Part > v2Part)
        return 1;
      if (v1Part < v2Part)
        return -1;
    }

    return 0;
  }

  private class GitHubRelease
  {
    public string TagName { get; set; } = string.Empty;
    public GitHubAsset[] Assets { get; set; } = Array.Empty<GitHubAsset>();
  }

  private class GitHubAsset
  {
    public string Name { get; set; } = string.Empty;
    public string BrowserDownloadUrl { get; set; } = string.Empty;
  }
}
