using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SpriteSheetCreator.App.ViewModels;
using SpriteSheetCreator.App.Views;
using SpriteSheetCreator.App.Services;
using SpriteSheetCreator.Core.Services;

namespace SpriteSheetCreator.App;

public partial class App : Application
{
    public new static App Current => (App)Application.Current;
    public IServiceProvider Services { get; }

    public App()
    {
        Services = ConfigureServices();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Core Services
        services.AddSingleton<IImageProcessor, ImageProcessor>();

        // App Services
        services.AddSingleton<IFileService, FileService>();

        // ViewModels
        services.AddSingleton<PreviewViewModel>();
        services.AddSingleton<ControlsViewModel>();
        services.AddSingleton<MainViewModel>();

        // Views
        services.AddTransient<MainWindow>();

        return services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.DataContext = Services.GetRequiredService<MainViewModel>();
        mainWindow.Show();
    }
}
