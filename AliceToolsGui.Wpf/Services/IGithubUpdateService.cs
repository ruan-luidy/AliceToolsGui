namespace AliceToolsGui.Wpf.Services;

public interface IGithubUpdateService
{
  Task<bool> CheckForUpdatesAsync();
  Task<string> GetLatestVersionAsync();
  Task DownloadAndInstallUpdateAsync(IProgress<int>? progress = null);
}
