using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AliceToolsGui.Wpf.Services;
using System.Collections.ObjectModel;

namespace AliceToolsGui.Wpf.ViewModels;

public partial class ArOperationsViewModel : ObservableObject
{
  private readonly IAliceToolsService _aliceToolsService;
  private readonly IFileDialogService _fileDialogService;
  private readonly IMessageBoxService _messageBoxService;

  [ObservableProperty]
  private string _archiveFilePath = string.Empty;

  [ObservableProperty]
  private string _extractOutputPath = string.Empty;

  [ObservableProperty]
  private string _packSourcePath = string.Empty;

  [ObservableProperty]
  private string _packOutputPath = string.Empty;

  [ObservableProperty]
  private ObservableCollection<string> _archiveContents = new();

  [ObservableProperty]
  private bool _isBusy;

  [ObservableProperty]
  private string _statusMessage = string.Empty;

  public ArOperationsViewModel(
      IAliceToolsService aliceToolsService,
      IFileDialogService fileDialogService,
      IMessageBoxService messageBoxService)
  {
    _aliceToolsService = aliceToolsService;
    _fileDialogService = fileDialogService;
    _messageBoxService = messageBoxService;
  }

  [RelayCommand]
  private async Task BrowseArchiveFile()
  {
    var path = await _fileDialogService.OpenFileDialogAsync(
        "Arquivos AR (*.afa;*.ald;*.dat)|*.afa;*.ald;*.dat|Todos os arquivos (*.*)|*.*");

    if (!string.IsNullOrEmpty(path))
    {
      ArchiveFilePath = path;
    }
  }

  [RelayCommand]
  private async Task BrowseExtractOutput()
  {
    var path = await _fileDialogService.OpenFolderDialogAsync();
    if (!string.IsNullOrEmpty(path))
    {
      ExtractOutputPath = path;
    }
  }

  [RelayCommand]
  private async Task BrowsePackSource()
  {
    var path = await _fileDialogService.OpenFolderDialogAsync();
    if (!string.IsNullOrEmpty(path))
    {
      PackSourcePath = path;
    }
  }

  [RelayCommand]
  private async Task BrowsePackOutput()
  {
    var path = await _fileDialogService.SaveFileDialogAsync(
        "Arquivos AR (*.afa;*.ald;*.dat)|*.afa;*.ald;*.dat");

    if (!string.IsNullOrEmpty(path))
    {
      PackOutputPath = path;
    }
  }

  [RelayCommand]
  private async Task ListArchiveContents()
  {
    if (string.IsNullOrEmpty(ArchiveFilePath))
    {
      await _messageBoxService.ShowWarningAsync(
          "Aviso",
          "Selecione um arquivo AR primeiro.");
      return;
    }

    try
    {
      IsBusy = true;
      StatusMessage = "Listando conteúdo do arquivo...";

      var result = await _aliceToolsService.ArListAsync(ArchiveFilePath);

      ArchiveContents.Clear();
      foreach (var item in result.Items)
      {
        ArchiveContents.Add(item);
      }

      StatusMessage = $"{ArchiveContents.Count} itens encontrados";
    }
    catch (Exception ex)
    {
      await _messageBoxService.ShowErrorAsync(
          "Erro ao listar arquivo",
          ex.Message);
      StatusMessage = "Erro";
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  private async Task ExtractArchive()
  {
    if (string.IsNullOrEmpty(ArchiveFilePath) || string.IsNullOrEmpty(ExtractOutputPath))
    {
      await _messageBoxService.ShowWarningAsync(
          "Aviso",
          "Preencha o arquivo AR e o caminho de saída.");
      return;
    }

    try
    {
      IsBusy = true;
      StatusMessage = "Extraindo arquivo...";

      await _aliceToolsService.ArExtractAsync(ArchiveFilePath, ExtractOutputPath);

      await _messageBoxService.ShowSuccessAsync(
          "Sucesso",
          "Arquivo extraído com sucesso!");

      StatusMessage = "Extração concluída";
    }
    catch (Exception ex)
    {
      await _messageBoxService.ShowErrorAsync(
          "Erro ao extrair arquivo",
          ex.Message);
      StatusMessage = "Erro";
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  private async Task PackArchive()
  {
    if (string.IsNullOrEmpty(PackSourcePath) || string.IsNullOrEmpty(PackOutputPath))
    {
      await _messageBoxService.ShowWarningAsync(
          "Aviso",
          "Preencha a pasta de origem e o arquivo de saída.");
      return;
    }

    try
    {
      IsBusy = true;
      StatusMessage = "Empacotando arquivo...";

      await _aliceToolsService.ArPackAsync(PackSourcePath, PackOutputPath);

      await _messageBoxService.ShowSuccessAsync(
          "Sucesso",
          "Arquivo empacotado com sucesso!");

      StatusMessage = "Empacotamento concluído";
    }
    catch (Exception ex)
    {
      await _messageBoxService.ShowErrorAsync(
          "Erro ao empacotar arquivo",
          ex.Message);
      StatusMessage = "Erro";
    }
    finally
    {
      IsBusy = false;
    }
  }
}
