using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AliceToolsGui.Wpf.Services;
using System.Collections.ObjectModel;
using System.Text;

namespace AliceToolsGui.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
  private readonly IAliceToolsService _aliceToolsService;
  private readonly IFileDialogService _fileDialogService;
  private readonly IMessageBoxService _messageBoxService;
  private readonly IGithubUpdateService _githubUpdateService;
  private readonly IEncodingService _encodingService;
  private readonly ILocalizationService _localizationService;

  [ObservableProperty]
  private string _statusMessage = string.Empty;

  [ObservableProperty]
  private string _footerMessage = string.Empty;

  [ObservableProperty]
  private bool _isBusy;

  [ObservableProperty]
  private int _selectedTabIndex;

  [ObservableProperty]
  private string _aliceToolsPath = string.Empty;

  [ObservableProperty]
  private Encoding? _selectedEncoding;

  [ObservableProperty]
  private ObservableCollection<Encoding> _availableEncodings = new();

  [ObservableProperty]
  private LanguageInfo? _selectedLanguage;

  [ObservableProperty]
  private ObservableCollection<LanguageInfo> _availableLanguages = new();

  // Child ViewModels
  [ObservableProperty]
  private ArOperationsViewModel _arOperationsViewModel;

  [ObservableProperty]
  private AinOperationsViewModel _ainOperationsViewModel;

  [ObservableProperty]
  private ExOperationsViewModel _exOperationsViewModel;

  [ObservableProperty]
  private AcxOperationsViewModel _acxOperationsViewModel;

  public MainViewModel(
      IAliceToolsService aliceToolsService,
      IFileDialogService fileDialogService,
      IMessageBoxService messageBoxService,
      IGithubUpdateService githubUpdateService,
      IEncodingService encodingService,
      ILocalizationService localizationService,
      ArOperationsViewModel arOperationsViewModel,
      AinOperationsViewModel ainOperationsViewModel,
      ExOperationsViewModel exOperationsViewModel,
      AcxOperationsViewModel acxOperationsViewModel)
  {
    _aliceToolsService = aliceToolsService;
    _fileDialogService = fileDialogService;
    _messageBoxService = messageBoxService;
    _githubUpdateService = githubUpdateService;
    _encodingService = encodingService;
    _localizationService = localizationService;
    _arOperationsViewModel = arOperationsViewModel;
    _ainOperationsViewModel = ainOperationsViewModel;
    _exOperationsViewModel = exOperationsViewModel;
    _acxOperationsViewModel = acxOperationsViewModel;

    LoadEncodings();
    LoadLanguages();
    LoadSettings();
    UpdateLocalizedStrings();

    // Atualizar strings quando o idioma mudar
    _localizationService.LanguageChanged += OnLanguageChanged;
  }

  private void LoadEncodings()
  {
    AvailableEncodings = new ObservableCollection<Encoding>(_encodingService.GetAvailableEncodings());
    SelectedEncoding = _encodingService.GetDefaultEncoding();
  }

  private void LoadLanguages()
  {
    AvailableLanguages = new ObservableCollection<LanguageInfo>(_localizationService.GetAvailableLanguages());
    SelectedLanguage = AvailableLanguages.FirstOrDefault(l => l.CultureName == _localizationService.CurrentCulture.Name);
  }

  private void LoadSettings()
  {
    // TODO: Load from configuration
    AliceToolsPath = Environment.GetEnvironmentVariable("ALICE_TOOLS_PATH") ?? string.Empty;
  }

  private void UpdateLocalizedStrings()
  {
    StatusMessage = _localizationService.GetString("Ready");
    FooterMessage = $"{_localizationService.GetString("AppTitle")} v1.0";
  }

  private void OnLanguageChanged(object? sender, EventArgs e)
  {
    UpdateLocalizedStrings();
    OnPropertyChanged(nameof(StatusMessage));
    OnPropertyChanged(nameof(FooterMessage));
  }

  [RelayCommand]
  private async Task CheckUpdates()
  {
    try
    {
      IsBusy = true;
      StatusMessage = _localizationService.GetString("CheckingUpdates");

      var hasUpdate = await _githubUpdateService.CheckForUpdatesAsync();

      if (hasUpdate)
      {
        var result = await _messageBoxService.ShowYesNoAsync(
            _localizationService.GetString("UpdateAvailable"),
            _localizationService.GetString("UpdateAvailableMessage"));

        if (result)
        {
          await _githubUpdateService.DownloadAndInstallUpdateAsync();
        }
      }
      else
      {
        await _messageBoxService.ShowInfoAsync(
            _localizationService.GetString("UpToDate"),
            _localizationService.GetString("UpToDateMessage"));
      }

      StatusMessage = _localizationService.GetString("Ready");
    }
    catch (Exception ex)
    {
      await _messageBoxService.ShowErrorAsync(
          _localizationService.GetString("Error"),
          ex.Message);
      StatusMessage = _localizationService.GetString("Error");
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  private async Task BrowseAliceToolsPath()
  {
    var path = await _fileDialogService.OpenFolderDialogAsync();
    if (!string.IsNullOrEmpty(path))
    {
      AliceToolsPath = path;
      Environment.SetEnvironmentVariable("ALICE_TOOLS_PATH", path);
    }
  }

  partial void OnSelectedEncodingChanged(Encoding? value)
  {
    if (value != null)
    {
      _encodingService.SetDefaultEncoding(value);
    }
  }

  partial void OnSelectedLanguageChanged(LanguageInfo? value)
  {
    if (value != null && value.CultureName != _localizationService.CurrentCulture.Name)
    {
      _localizationService.SetLanguage(value.CultureName);
    }
  }
}
