using System.Globalization;
using System.Resources;

namespace AliceToolsGui.Wpf.Services;

public class LocalizationService : ILocalizationService
{
  private readonly ResourceManager _resourceManager;
  private CultureInfo _currentCulture;

  public event EventHandler? LanguageChanged;

  public CultureInfo CurrentCulture => _currentCulture;

  public LocalizationService()
  {
    // Resource manager aponta para os arquivos .resx
    _resourceManager = new ResourceManager(
        "AliceToolsGui.Wpf.Resources.Localization.Strings",
        typeof(LocalizationService).Assembly);

    // Tenta carregar a cultura do sistema, senão usa inglês
    var systemCulture = CultureInfo.CurrentUICulture.Name;
    _currentCulture = IsLanguageSupported(systemCulture)
        ? new CultureInfo(systemCulture)
        : new CultureInfo("en-US");

    Thread.CurrentThread.CurrentUICulture = _currentCulture;
    Thread.CurrentThread.CurrentCulture = _currentCulture;
  }

  public string GetString(string key)
  {
    try
    {
      var value = _resourceManager.GetString(key, _currentCulture);
      return value ?? $"[{key}]";
    }
    catch
    {
      return $"[{key}]";
    }
  }

  public string GetString(string key, params object[] args)
  {
    try
    {
      var format = GetString(key);
      return string.Format(format, args);
    }
    catch
    {
      return $"[{key}]";
    }
  }

  public void SetLanguage(string cultureName)
  {
    if (!IsLanguageSupported(cultureName))
    {
      throw new ArgumentException($"Language '{cultureName}' is not supported.");
    }

    _currentCulture = new CultureInfo(cultureName);
    Thread.CurrentThread.CurrentUICulture = _currentCulture;
    Thread.CurrentThread.CurrentCulture = _currentCulture;

    LanguageChanged?.Invoke(this, EventArgs.Empty);
  }

  public List<LanguageInfo> GetAvailableLanguages()
  {
    return new List<LanguageInfo>
        {
            new LanguageInfo
            {
                CultureName = "en-US",
                DisplayName = "English",
                NativeName = "English"
            },
            new LanguageInfo
            {
                CultureName = "zh-CN",
                DisplayName = "Chinese (Simplified)",
                NativeName = "简体中文"
            }
        };
  }

  private bool IsLanguageSupported(string cultureName)
  {
    var supportedLanguages = new[] { "en-US", "zh-CN" };

    // Verifica cultura exata
    if (supportedLanguages.Contains(cultureName))
      return true;

    // Verifica apenas o idioma (ex: "zh" para "zh-CN")
    var language = cultureName.Split('-')[0];
    return supportedLanguages.Any(l => l.StartsWith(language));
  }
}
