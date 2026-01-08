using System.Diagnostics;
using System.IO;
using System.Text;

namespace AliceToolsGui.Wpf.Services;

public class AliceToolsService : IAliceToolsService
{
  private readonly string _aliceToolsPath;

  public AliceToolsService()
  {
    _aliceToolsPath = Environment.GetEnvironmentVariable("ALICE_TOOLS_PATH")
        ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "alice-tools");
  }

  private async Task<string> RunAliceToolAsync(string toolName, string arguments)
  {
    var toolPath = Path.Combine(_aliceToolsPath, $"{toolName}.exe");

    if (!File.Exists(toolPath))
    {
      throw new FileNotFoundException($"Alice Tools executável não encontrado: {toolPath}");
    }

    var startInfo = new ProcessStartInfo
    {
      FileName = toolPath,
      Arguments = arguments,
      UseShellExecute = false,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      CreateNoWindow = true,
      StandardOutputEncoding = Encoding.UTF8,
      StandardErrorEncoding = Encoding.UTF8
    };

    using var process = new Process { StartInfo = startInfo };

    var outputBuilder = new StringBuilder();
    var errorBuilder = new StringBuilder();

    process.OutputDataReceived += (sender, e) =>
    {
      if (e.Data != null)
        outputBuilder.AppendLine(e.Data);
    };

    process.ErrorDataReceived += (sender, e) =>
    {
      if (e.Data != null)
        errorBuilder.AppendLine(e.Data);
    };

    process.Start();
    process.BeginOutputReadLine();
    process.BeginErrorReadLine();

    await process.WaitForExitAsync();

    var output = outputBuilder.ToString();
    var error = errorBuilder.ToString();

    if (process.ExitCode != 0)
    {
      throw new Exception($"Alice Tools retornou erro: {error}");
    }

    return output;
  }

  public async Task<ArListResult> ArListAsync(string archivePath)
  {
    var output = await RunAliceToolAsync("alice-ar", $"list \"{archivePath}\"");

    var result = new ArListResult
    {
      RawOutput = output,
      Items = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList()
    };

    return result;
  }

  public async Task ArExtractAsync(string archivePath, string outputPath)
  {
    await RunAliceToolAsync("alice-ar", $"extract \"{archivePath}\" -o \"{outputPath}\"");
  }

  public async Task ArPackAsync(string sourcePath, string outputPath)
  {
    await RunAliceToolAsync("alice-ar", $"pack \"{sourcePath}\" -o \"{outputPath}\"");
  }

  public async Task<string> AinDumpAsync(string ainPath, string outputPath)
  {
    return await RunAliceToolAsync("alice-ain", $"dump \"{ainPath}\" -o \"{outputPath}\"");
  }

  public async Task AinEditAsync(string ainPath)
  {
    await RunAliceToolAsync("alice-ain", $"edit \"{ainPath}\"");
  }

  public async Task<string> AinCompareAsync(string file1, string file2)
  {
    return await RunAliceToolAsync("alice-ain", $"compare \"{file1}\" \"{file2}\"");
  }

  public async Task<string> ExDumpAsync(string exPath, string outputPath)
  {
    return await RunAliceToolAsync("alice-ex", $"dump \"{exPath}\" -o \"{outputPath}\"");
  }

  public async Task ExBuildAsync(string sourcePath, string outputPath)
  {
    await RunAliceToolAsync("alice-ex", $"build \"{sourcePath}\" -o \"{outputPath}\"");
  }

  public async Task<string> ExCompareAsync(string file1, string file2)
  {
    return await RunAliceToolAsync("alice-ex", $"compare \"{file1}\" \"{file2}\"");
  }

  public async Task<string> AcxDumpAsync(string acxPath, string outputPath)
  {
    return await RunAliceToolAsync("alice-acx", $"dump \"{acxPath}\" -o \"{outputPath}\"");
  }

  public async Task AcxBuildAsync(string sourcePath, string outputPath)
  {
    await RunAliceToolAsync("alice-acx", $"build \"{sourcePath}\" -o \"{outputPath}\"");
  }
}
