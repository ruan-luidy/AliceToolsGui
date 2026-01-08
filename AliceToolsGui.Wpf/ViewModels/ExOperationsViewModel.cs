using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AliceToolsGui.Wpf.Services;

namespace AliceToolsGui.Wpf.ViewModels;

public partial class ExOperationsViewModel : ObservableObject
{
  private readonly IAliceToolsService _aliceToolsService;
  private readonly IFileDialogService _fileDialogService;
  private readonly IMessageBoxService _messageBoxService;

  [ObservableProperty]
  private string _exFilePath = string.Empty;

  [ObservableProperty]
  private string _dumpOutputPath = string.Empty;

  [ObservableProperty]
  private string _buildSourcePath = string.Empty;

  [ObservableProperty]
  private string _buildOutputPath = string.Empty;

  [ObservableProperty]
  private string _compareFilePath = string.Empty;

  [ObservableProperty]
  private string _compareWithFilePath = string.Empty;

  [ObservableProperty]
  private bool _isBusy;

  [ObservableProperty]
  private string _statusMessage = string.Empty;

  [ObservableProperty]
  private string _result = string.Empty;

  public ExOperationsViewModel(
      IAliceToolsService aliceToolsService,
      IFileDialogService fileDialogService,
      IMessageBoxService messageBoxService)
  {
    _aliceToolsService = aliceToolsService;
    _fileDialogService = fileDialogService;
    _messageBoxService = messageBoxService;
  }

  [RelayCommand]
  private async Task BrowseExFile()
  {
    var path = await _fileDialogService.OpenFileDialogAsync(
        "Arquivos EX (*.ex)|*.ex|Todos os arquivos (*.*)|*.*");

    if (!string.IsNullOrEmpty(path))
    {
      ExFilePath = path;
    }
  }

  [RelayCommand]
  private async Task BrowseDumpOutput()
  {
    var path = await _fileDialogService.OpenFolderDialogAsync();
    if (!string.IsNullOrEmpty(path))
    {
      DumpOutputPath = path;
    }
  }

  [RelayCommand]
  private async Task BrowseBuildSource()
  {
    var path = await _fileDialogService.OpenFolderDialogAsync();
    if (!string.IsNullOrEmpty(path))
    {
      BuildSourcePath = path;
    }
  }

  [RelayCommand]
  private async Task BrowseBuildOutput()
  {
    var path = await _fileDialogService.SaveFileDialogAsync(
        "Arquivos EX (*.ex)|*.ex");

    if (!string.IsNullOrEmpty(path))
    {
      BuildOutputPath = path;
    }
  }

  [RelayCommand]
  private async Task DumpEx()
  {
    if (string.IsNullOrEmpty(ExFilePath) || string.IsNullOrEmpty(DumpOutputPath))
    {
      await _messageBoxService.ShowWarningAsync(
          "Aviso",
          "Preencha o arquivo EX e o caminho de saída.");
      return;
    }

    try
    {
      IsBusy = true;
      StatusMessage = "Fazendo dump do arquivo EX...";

      var result = await _aliceToolsService.ExDumpAsync(ExFilePath, DumpOutputPath);

      Result = result;

      await _messageBoxService.ShowSuccessAsync(
          "Sucesso",
          "Dump realizado com sucesso!");

      StatusMessage = "Dump concluído";
    }
    catch (Exception ex)
    {
      await _messageBoxService.ShowErrorAsync(
          "Erro ao fazer dump",
          ex.Message);
      StatusMessage = "Erro";
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  private async Task BuildEx()
  {
    if (string.IsNullOrEmpty(BuildSourcePath) || string.IsNullOrEmpty(BuildOutputPath))
    {
      await _messageBoxService.ShowWarningAsync(
          "Aviso",
          "Preencha a pasta de origem e o arquivo de saída.");
      return;
    }

    try
    {
      IsBusy = true;
      StatusMessage = "Construindo arquivo EX...";

      await _aliceToolsService.ExBuildAsync(BuildSourcePath, BuildOutputPath);

      await _messageBoxService.ShowSuccessAsync(
          "Sucesso",
          "Arquivo EX construído com sucesso!");

      StatusMessage = "Construção concluída";
    }
    catch (Exception ex)
    {
      await _messageBoxService.ShowErrorAsync(
          "Erro ao construir arquivo",
          ex.Message);
      StatusMessage = "Erro";
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  private async Task CompareEx()
  {
    if (string.IsNullOrEmpty(CompareFilePath) || string.IsNullOrEmpty(CompareWithFilePath))
    {
      await _messageBoxService.ShowWarningAsync(
          "Aviso",
          "Selecione ambos os arquivos para comparação.");
      return;
    }

    try
    {
      IsBusy = true;
      StatusMessage = "Comparando arquivos EX...";

      var result = await _aliceToolsService.ExCompareAsync(CompareFilePath, CompareWithFilePath);

      Result = result;

      StatusMessage = "Comparação concluída";
    }
    catch (Exception ex)
    {
      await _messageBoxService.ShowErrorAsync(
          "Erro ao comparar arquivos",
          ex.Message);
      StatusMessage = "Erro";
    }
    finally
    {
      IsBusy = false;
    }
  }
}
