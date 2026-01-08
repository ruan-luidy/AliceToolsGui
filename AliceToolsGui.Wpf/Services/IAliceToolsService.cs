namespace AliceToolsGui.Wpf.Services;

public interface IAliceToolsService
{
  // AR Operations
  Task<ArListResult> ArListAsync(string archivePath);
  Task ArExtractAsync(string archivePath, string outputPath);
  Task ArPackAsync(string sourcePath, string outputPath);

  // AIN Operations
  Task<string> AinDumpAsync(string ainPath, string outputPath);
  Task AinEditAsync(string ainPath);
  Task<string> AinCompareAsync(string file1, string file2);

  // EX Operations
  Task<string> ExDumpAsync(string exPath, string outputPath);
  Task ExBuildAsync(string sourcePath, string outputPath);
  Task<string> ExCompareAsync(string file1, string file2);

  // ACX Operations
  Task<string> AcxDumpAsync(string acxPath, string outputPath);
  Task AcxBuildAsync(string sourcePath, string outputPath);
}

public class ArListResult
{
  public List<string> Items { get; set; } = new();
  public string RawOutput { get; set; } = string.Empty;
}
