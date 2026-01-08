using Microsoft.Win32;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using DialogResult = System.Windows.Forms.DialogResult;

namespace AliceToolsGui.Wpf.Services;

public class FileDialogService : IFileDialogService
{
  public Task<string> OpenFileDialogAsync(string filter = "Todos os arquivos (*.*)|*.*")
  {
    var dialog = new OpenFileDialog
    {
      Filter = filter,
      CheckFileExists = true,
      CheckPathExists = true
    };

    var result = dialog.ShowDialog();
    return Task.FromResult(result == true ? dialog.FileName : string.Empty);
  }

  public Task<string> SaveFileDialogAsync(string filter = "Todos os arquivos (*.*)|*.*")
  {
    var dialog = new SaveFileDialog
    {
      Filter = filter,
      CheckPathExists = true
    };

    var result = dialog.ShowDialog();
    return Task.FromResult(result == true ? dialog.FileName : string.Empty);
  }

  public Task<string> OpenFolderDialogAsync()
  {
    using var dialog = new FolderBrowserDialog
    {
      Description = "Selecione uma pasta",
      ShowNewFolderButton = true
    };

    var result = dialog.ShowDialog();
    return Task.FromResult(result == DialogResult.OK ? dialog.SelectedPath : string.Empty);
  }
}
