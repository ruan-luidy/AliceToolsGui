namespace AliceToolsGui.Wpf.Services;

public interface IMessageBoxService
{
  Task ShowInfoAsync(string title, string message);
  Task ShowSuccessAsync(string title, string message);
  Task ShowWarningAsync(string title, string message);
  Task ShowErrorAsync(string title, string message);
  Task<bool> ShowYesNoAsync(string title, string message);
}
