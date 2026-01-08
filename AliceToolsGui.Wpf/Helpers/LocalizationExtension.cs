using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using AliceToolsGui.Wpf.Services;

namespace AliceToolsGui.Wpf.Helpers;

/// <summary>
/// MarkupExtension para binding de strings localizadas no XAML
/// Uso: Text="{loc:Localization Key=AppTitle}"
/// </summary>
public class LocalizationExtension : MarkupExtension
{
  public string Key { get; set; } = string.Empty;

  public LocalizationExtension()
  {
  }

  public LocalizationExtension(string key)
  {
    Key = key;
  }

  public override object ProvideValue(IServiceProvider serviceProvider)
  {
    if (string.IsNullOrEmpty(Key))
      return $"[No Key]";

    var binding = new Binding("Value")
    {
      Source = new LocalizedString(Key),
      Mode = System.Windows.Data.BindingMode.OneWay
    };

    return binding.ProvideValue(serviceProvider);
  }
}

/// <summary>
/// Wrapper que monitora mudanças de idioma e atualiza o binding
/// </summary>
public class LocalizedString : INotifyPropertyChanged
{
  private readonly string _key;
  private static ILocalizationService? _localizationService;
  private static event PropertyChangedEventHandler? StaticPropertyChanged;

  public event PropertyChangedEventHandler? PropertyChanged;

  public string Value => _localizationService?.GetString(_key) ?? $"[{_key}]";

  static LocalizedString()
  {
    // O serviço será injetado pela aplicação
    var app = Application.Current as App;
    if (app != null)
    {
      _localizationService = App.GetService<ILocalizationService>();
      if (_localizationService != null)
      {
        _localizationService.LanguageChanged += OnLanguageChanged;
      }
    }
  }

  public LocalizedString(string key)
  {
    _key = key;
    StaticPropertyChanged += OnStaticPropertyChanged;
  }

  private void OnStaticPropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    PropertyChanged?.Invoke(this, e);
  }

  private static void OnLanguageChanged(object? sender, EventArgs e)
  {
    // Notifica todas as instâncias para atualizar
    StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Value)));
  }

  private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}
