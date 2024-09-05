using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ImageFanReloaded.Bootstrap;

namespace ImageFanReloaded;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
	    var desktop = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;

        IAppBootstrap appBootstrap = new AppBootstrap(desktop);
	    await appBootstrap.BootstrapApplication();
    }
}
