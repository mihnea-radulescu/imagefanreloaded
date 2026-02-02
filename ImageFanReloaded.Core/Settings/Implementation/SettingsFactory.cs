using System.IO;
using ImageFanReloaded.Core.RuntimeEnvironment;

namespace ImageFanReloaded.Core.Settings.Implementation;

public class SettingsFactory : ISettingsFactory
{
	public ITabOptions GetTabOptions()
		=> new TabOptions(_defaultTabOptions);

	public IThumbnailCacheOptions GetThumbnailCacheOptions()
		=> new ThumbnailCacheOptions(_configFolderPath);

	public SettingsFactory(IGlobalParameters globalParameters)
	{
		_configFolderPath = GetConfigFolderPath(globalParameters);

		_defaultTabOptions = new TabOptions(_configFolderPath);
	}

	private readonly string _configFolderPath;

	private readonly TabOptions _defaultTabOptions;

	private static string GetConfigFolderPath(
		IGlobalParameters globalParameters)
	{
		string configFolderPath;

		if (globalParameters.RuntimeEnvironmentType ==
			RuntimeEnvironmentType.Linux)
		{
			configFolderPath = Path.Combine(
				globalParameters.UserHomePath,
				".config",
				globalParameters.ApplicationName);
		}
		else if (globalParameters.RuntimeEnvironmentType ==
				 RuntimeEnvironmentType.LinuxFlatpak)
		{
			configFolderPath = globalParameters.UserConfigPath;
		}
		else
		{
			configFolderPath = Path.Combine(
				globalParameters.UserConfigPath,
				globalParameters.ApplicationName);
		}

		return configFolderPath;
	}
}
