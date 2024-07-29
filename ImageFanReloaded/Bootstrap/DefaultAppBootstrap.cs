using Avalonia.Controls.ApplicationLifetimes;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Settings;

namespace ImageFanReloaded.Bootstrap;

public class DefaultAppBootstrap : AppBootstrapBase
{
    public DefaultAppBootstrap(IClassicDesktopStyleApplicationLifetime desktop) : base(desktop)
    {
    }
    
    #region Protected

    protected override string? GetInputPath()
    {
        var args = Desktop.Args!;
        var inputPath = args.Length > 0 ? args[0] : null;

        return inputPath;
    }

    protected override IAppSettings GetAppSettings() => new DefaultAppSettings();
    
    #endregion
}
