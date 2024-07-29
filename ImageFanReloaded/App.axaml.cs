//#define FLATPAK

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
        
	    IAppBootstrap appBootstrap;
            
        #if FLATPAK
            appBootstrap = new FlatpakAppBootstrap(desktop);
        #else
            appBootstrap = new DefaultAppBootstrap(desktop);
        #endif

	    await appBootstrap.BootstrapApplication();
    }
}
