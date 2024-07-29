using Avalonia.Controls.ApplicationLifetimes;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Settings;

namespace ImageFanReloaded.Bootstrap;

public class FlatpakAppBootstrap : AppBootstrapBase
{
    public FlatpakAppBootstrap(IClassicDesktopStyleApplicationLifetime desktop) : base(desktop)
    {
    }
    
    #region Protected

    protected override string? GetInputPath() => null;

    protected override IAppSettings GetAppSettings() => new FlatpakAppSettings();
    
    #endregion
}
