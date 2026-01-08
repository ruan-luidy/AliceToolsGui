namespace AliceToolsGui.Wpf.Services;

public interface IFileDialogService
{
  Task<string> OpenFileDialogAsync(string filter = "Todos os arquivos (*.*)|*.*");
  Task<string> SaveFileDialogAsync(string filter = "Todos os arquivos (*.*)|*.*");
  Task<string> OpenFolderDialogAsync();
}
