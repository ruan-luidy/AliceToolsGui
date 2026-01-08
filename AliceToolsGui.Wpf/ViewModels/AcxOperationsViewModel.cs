using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AliceToolsGui.Wpf.Services;

namespace AliceToolsGui.Wpf.ViewModels;

public partial class AcxOperationsViewModel : ObservableObject
{
  private readonly IAliceToolsService _aliceToolsService;
  private readonly IFileDialogService _fileDialogService;
  private readonly IMessageBoxService _messageBoxService;

  [ObservableProperty]
  private string _acxFilePath = string.Empty;

  [ObservableProperty]
  private string _dumpOutputPath = string.Empty;

  [ObservableProperty]
  private string _buildSourcePath = string.Empty;

  [ObservableProperty]
  private string _buildOutputPath = string.Empty;

  [ObservableProperty]
  private bool _isBusy;

  [ObservableProperty]
  private string _statusMessage = string.Empty;

  [ObservableProperty]
  private string _result = string.Empty;

  public AcxOperationsViewModel(
      IAliceToolsService aliceToolsService,
      IFileDialogService fileDialogService,
      IMessageBoxService messageBoxService)
  {
    _aliceToolsService = aliceToolsService;
    _fileDialogService = fileDialogService;
    _messageBoxService = messageBoxService;
  }

  [RelayCommand]
  private async Task BrowseAcxFile()
  {
    var path = await _fileDialogService.OpenFileDialogAsync(
        "Arquivos ACX (*.acx)|*.acx|Todos os arquivos (*.*)|*.*");

    if (!string.IsNullOrEmpty(path))
    {
      AcxFilePath = path;
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
        "Arquivos ACX (*.acx)|*.acx");

    if (!string.IsNullOrEmpty(path))
    {
      BuildOutputPath = path;
    }
  }

  [RelayCommand]
  private async Task DumpAcx()
  {
    if (string.IsNullOrEmpty(AcxFilePath) || string.IsNullOrEmpty(DumpOutputPath))
    {
      await _messageBoxService.ShowWarningAsync(
          "Aviso",
          "Preencha o arquivo ACX e o caminho de saída.");
      return;
    }

    try
    {
      IsBusy = true;
      StatusMessage = "Fazendo dump do arquivo ACX...";

      var result = await _aliceToolsService.AcxDumpAsync(AcxFilePath, DumpOutputPath);

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
  private async Task BuildAcx()
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
      StatusMessage = "Construindo arquivo ACX...";

      await _aliceToolsService.AcxBuildAsync(BuildSourcePath, BuildOutputPath);

      await _messageBoxService.ShowSuccessAsync(
          "Sucesso",
          "Arquivo ACX construído com sucesso!");

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
}
