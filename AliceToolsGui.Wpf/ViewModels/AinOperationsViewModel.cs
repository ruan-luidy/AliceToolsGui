using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AliceToolsGui.Wpf.Services;

namespace AliceToolsGui.Wpf.ViewModels;

public partial class AinOperationsViewModel : ObservableObject
{
  private readonly IAliceToolsService _aliceToolsService;
  private readonly IFileDialogService _fileDialogService;
  private readonly IMessageBoxService _messageBoxService;

  [ObservableProperty]
  private string _ainFilePath = string.Empty;

  [ObservableProperty]
  private string _dumpOutputPath = string.Empty;

  [ObservableProperty]
  private string _sourceFilePath = string.Empty;

  [ObservableProperty]
  private string _compareFilePath = string.Empty;

  [ObservableProperty]
  private string _compareWithFilePath = string.Empty;

  [ObservableProperty]
  private bool _isBusy;

  [ObservableProperty]
  private string _statusMessage = string.Empty;

  [ObservableProperty]
  private string _dumpResult = string.Empty;

  public AinOperationsViewModel(
      IAliceToolsService aliceToolsService,
      IFileDialogService fileDialogService,
      IMessageBoxService messageBoxService)
  {
    _aliceToolsService = aliceToolsService;
    _fileDialogService = fileDialogService;
    _messageBoxService = messageBoxService;
  }

  [RelayCommand]
  private async Task BrowseAinFile()
  {
    var path = await _fileDialogService.OpenFileDialogAsync(
        "Arquivos AIN (*.ain)|*.ain|Todos os arquivos (*.*)|*.*");

    if (!string.IsNullOrEmpty(path))
    {
      AinFilePath = path;
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
  private async Task BrowseSourceFile()
  {
    var path = await _fileDialogService.OpenFileDialogAsync(
        "Arquivos JAM (*.jam)|*.jam|Todos os arquivos (*.*)|*.*");

    if (!string.IsNullOrEmpty(path))
    {
      SourceFilePath = path;
    }
  }

  [RelayCommand]
  private async Task DumpAin()
  {
    if (string.IsNullOrEmpty(AinFilePath) || string.IsNullOrEmpty(DumpOutputPath))
    {
      await _messageBoxService.ShowWarningAsync(
          "Aviso",
          "Preencha o arquivo AIN e o caminho de saída.");
      return;
    }

    try
    {
      IsBusy = true;
      StatusMessage = "Fazendo dump do arquivo AIN...";

      var result = await _aliceToolsService.AinDumpAsync(AinFilePath, DumpOutputPath);

      DumpResult = result;

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
  private async Task EditAin()
  {
    if (string.IsNullOrEmpty(AinFilePath))
    {
      await _messageBoxService.ShowWarningAsync(
          "Aviso",
          "Selecione um arquivo AIN.");
      return;
    }

    try
    {
      IsBusy = true;
      StatusMessage = "Editando arquivo AIN...";

      await _aliceToolsService.AinEditAsync(AinFilePath);

      await _messageBoxService.ShowSuccessAsync(
          "Sucesso",
          "Arquivo AIN editado com sucesso!");

      StatusMessage = "Edição concluída";
    }
    catch (Exception ex)
    {
      await _messageBoxService.ShowErrorAsync(
          "Erro ao editar arquivo",
          ex.Message);
      StatusMessage = "Erro";
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  private async Task CompareAin()
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
      StatusMessage = "Comparando arquivos AIN...";

      var result = await _aliceToolsService.AinCompareAsync(CompareFilePath, CompareWithFilePath);

      DumpResult = result;

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
