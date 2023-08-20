using Avalonia;
using Avalonia.Markup.Xaml;

namespace ImageFanReloaded.Avalonia.Tests;

public partial class TestApp
    : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();
    }
}
