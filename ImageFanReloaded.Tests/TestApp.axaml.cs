using Avalonia;
using Avalonia.Markup.Xaml;

namespace ImageFanReloaded.Tests;

public class TestApp
    : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
