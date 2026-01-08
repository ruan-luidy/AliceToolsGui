using System.Globalization;

namespace AliceToolsGui.Wpf.Services;

public interface ILocalizationService
{
  string GetString(string key);
  string GetString(string key, params object[] args);
  void SetLanguage(string cultureName);
  CultureInfo CurrentCulture { get; }
  List<LanguageInfo> GetAvailableLanguages();
  event EventHandler? LanguageChanged;
}

public class LanguageInfo
{
  public string CultureName { get; set; } = string.Empty;
  public string DisplayName { get; set; } = string.Empty;
  public string NativeName { get; set; } = string.Empty;
}
