using ImageFanReloaded.Core.RuntimeEnvironment;

namespace ImageFanReloaded.Core.Settings.Implementation;

public class SettingsFactory : ISettingsFactory
{
	public ITabOptions GetTabOptions() => new TabOptions();

	public SettingsFactory(IGlobalParameters globalParameters)
	{
		string configFolderName;

		if (globalParameters.RuntimeEnvironmentType is
			RuntimeEnvironmentType.LinuxFlatpak)
		{
			configFolderName = string.Empty;
		}
		else
		{
			configFolderName = ApplicationName;
		}

		TabOptions.LoadDefaultTabOptions(configFolderName);
	}

	private const string ApplicationName = "ImageFanReloaded";
}
