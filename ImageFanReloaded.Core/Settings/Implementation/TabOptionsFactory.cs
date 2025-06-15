namespace ImageFanReloaded.Core.Settings.Implementation;

public class TabOptionsFactory : ITabOptionsFactory
{
	public ITabOptions GetTabOptions() => new TabOptions();
}
