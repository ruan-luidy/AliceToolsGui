using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using AliceToolsGui.Wpf.Services;
using AliceToolsGui.Wpf.ViewModels;
using AliceToolsGui.Wpf.Views;

namespace AliceToolsGui.Wpf;

public partial class App : Application
{
  private readonly IHost _host;

  public App()
  {
    _host = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((context, config) =>
        {
          config.AddEnvironmentVariables();
          config.AddUserSecrets<App>(optional: true);
        })
        .ConfigureServices((context, services) =>
        {
          // Register Configuration
          services.AddSingleton(context.Configuration);

          // Register MediatR
          services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(App).Assembly));

          // Register Services
          services.AddSingleton<ILocalizationService, LocalizationService>();
          services.AddSingleton<IAliceToolsService, AliceToolsService>();
          services.AddSingleton<IFileDialogService, FileDialogService>();
          services.AddSingleton<IMessageBoxService, MessageBoxService>();
          services.AddSingleton<IGithubUpdateService, GithubUpdateService>();
          services.AddSingleton<IEncodingService, EncodingService>();

          // Register ViewModels
          services.AddTransient<MainViewModel>();
          services.AddTransient<ArOperationsViewModel>();
          services.AddTransient<AinOperationsViewModel>();
          services.AddTransient<ExOperationsViewModel>();
          services.AddTransient<AcxOperationsViewModel>();
          services.AddTransient<UpdateViewModel>();

          // Register Windows
          services.AddSingleton<MainWindow>();
        })
        .Build();
  }

  protected override async void OnStartup(StartupEventArgs e)
  {
    await _host.StartAsync();

    var mainWindow = _host.Services.GetRequiredService<MainWindow>();
    mainWindow.DataContext = _host.Services.GetRequiredService<MainViewModel>();
    mainWindow.Show();

    base.OnStartup(e);
  }

  protected override async void OnExit(ExitEventArgs e)
  {
    using (_host)
    {
      await _host.StopAsync();
    }

    base.OnExit(e);
  }

  public static T GetService<T>() where T : class
  {
    return ((App)Current)._host.Services.GetRequiredService<T>();
  }
}
