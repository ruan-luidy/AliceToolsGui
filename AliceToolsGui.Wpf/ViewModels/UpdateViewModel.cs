using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AliceToolsGui.Wpf.Services;

namespace AliceToolsGui.Wpf.ViewModels;

public partial class UpdateViewModel : ObservableObject
{
  private readonly IGithubUpdateService _githubUpdateService;
  private readonly IMessageBoxService _messageBoxService;

  [ObservableProperty]
  private string _currentVersion = "1.0.0";

  [ObservableProperty]
  private string _latestVersion = string.Empty;

  [ObservableProperty]
  private bool _hasUpdate;

  [ObservableProperty]
  private bool _isBusy;

  [ObservableProperty]
  private string _statusMessage = string.Empty;

  [ObservableProperty]
  private int _downloadProgress;

  public UpdateViewModel(
      IGithubUpdateService githubUpdateService,
      IMessageBoxService messageBoxService)
  {
    _githubUpdateService = githubUpdateService;
    _messageBoxService = messageBoxService;
  }

  [RelayCommand]
  private async Task CheckForUpdates()
  {
    try
    {
      IsBusy = true;
      StatusMessage = "Verificando atualizações...";

      HasUpdate = await _githubUpdateService.CheckForUpdatesAsync();

      if (HasUpdate)
      {
        LatestVersion = await _githubUpdateService.GetLatestVersionAsync();
        StatusMessage = $"Nova versão disponível: {LatestVersion}";
      }
      else
      {
        StatusMessage = "Você está usando a versão mais recente.";
      }
    }
    catch (Exception ex)
    {
      await _messageBoxService.ShowErrorAsync(
          "Erro ao verificar atualizações",
          ex.Message);
      StatusMessage = "Erro ao verificar atualizações";
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  private async Task DownloadUpdate()
  {
    try
    {
      IsBusy = true;
      StatusMessage = "Baixando atualização...";

      var progress = new Progress<int>(value =>
      {
        DownloadProgress = value;
        StatusMessage = $"Baixando atualização... {value}%";
      });

      await _githubUpdateService.DownloadAndInstallUpdateAsync(progress);

      await _messageBoxService.ShowSuccessAsync(
          "Sucesso",
          "Atualização baixada com sucesso! O aplicativo será reiniciado.");

      StatusMessage = "Atualização concluída";
    }
    catch (Exception ex)
    {
      await _messageBoxService.ShowErrorAsync(
          "Erro ao baixar atualização",
          ex.Message);
      StatusMessage = "Erro ao baixar atualização";
    }
    finally
    {
      IsBusy = false;
    }
  }
}
