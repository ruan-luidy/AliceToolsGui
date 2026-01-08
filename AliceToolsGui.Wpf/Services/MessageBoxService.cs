using HandyControl.Controls;
using HandyControl.Data;
using System.Windows;
using MessageBox = HandyControl.Controls.MessageBox;


namespace AliceToolsGui.Wpf.Services;

public class MessageBoxService : IMessageBoxService
{
  public Task ShowInfoAsync(string title, string message)
  {
    MessageBox.Show(new MessageBoxInfo
    {
      Message = message,
      Caption = title,
      Button = MessageBoxButton.OK,
      IconBrushKey = ResourceToken.AccentBrush,
      IconKey = ResourceToken.InfoGeometry
    });

    return Task.CompletedTask;
  }

  public Task ShowSuccessAsync(string title, string message)
  {
    MessageBox.Show(new MessageBoxInfo
    {
      Message = message,
      Caption = title,
      Button = MessageBoxButton.OK,
      IconBrushKey = ResourceToken.SuccessBrush,
      IconKey = ResourceToken.SuccessGeometry
    });

    return Task.CompletedTask;
  }

  public Task ShowWarningAsync(string title, string message)
  {
    MessageBox.Show(new MessageBoxInfo
    {
      Message = message,
      Caption = title,
      Button = MessageBoxButton.OK,
      IconBrushKey = ResourceToken.WarningBrush,
      IconKey = ResourceToken.WarningGeometry
    });

    return Task.CompletedTask;
  }

  public Task ShowErrorAsync(string title, string message)
  {
    MessageBox.Show(new MessageBoxInfo
    {
      Message = message,
      Caption = title,
      Button = MessageBoxButton.OK,
      IconBrushKey = ResourceToken.DangerBrush,
      IconKey = ResourceToken.ErrorGeometry
    });

    return Task.CompletedTask;
  }

  public Task<bool> ShowYesNoAsync(string title, string message)
  {
    var result = MessageBox.Show(new MessageBoxInfo
    {
      Message = message,
      Caption = title,
      Button = MessageBoxButton.YesNo,
      IconBrushKey = ResourceToken.AccentBrush,
      IconKey = ResourceToken.AskGeometry
    });

    return Task.FromResult(result == MessageBoxResult.Yes);
  }
}
