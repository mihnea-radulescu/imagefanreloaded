namespace ImageFanReloaded.Core.Settings.Implementation;

public abstract class TabOptionsFactoryBase : ITabOptionsFactory
{
	public ITabOptions GetTabOptions() => new TabOptions();

	protected TabOptionsFactoryBase()
	{
		TabOptions.LoadDefaultTabOptions(SettingsFolderName);
	}

	protected abstract string SettingsFolderName { get; }
}
