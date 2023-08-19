using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ImageFanReloaded.Views.Implementation;

namespace ImageFanReloaded;

public partial class App
    : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var desktop = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime;
		desktop.MainWindow = new MainWindow();

        base.OnFrameworkInitializationCompleted();
    }
}
