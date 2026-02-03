using System.IO;
using ImageFanReloaded.Core.RuntimeEnvironment;

namespace ImageFanReloaded.Core.Settings.Implementation;

public class SettingsFactory : ISettingsFactory
{
	public ITabOptions GetTabOptions()
		=> new TabOptions(_defaultTabOptions);

	public IThumbnailCacheOptions GetThumbnailCacheOptions()
		=> new ThumbnailCacheOptions(_configFolderPath);

	public string GetCacheFolderPath() => _cacheFolderPath;

	public SettingsFactory(IGlobalParameters globalParameters)
	{
		_configFolderPath = GetConfigFolderPath(globalParameters);
		_cacheFolderPath = GetCacheFolderPath(globalParameters);

		_defaultTabOptions = new TabOptions(_configFolderPath);
	}

	private readonly string _configFolderPath;
	private readonly string _cacheFolderPath;

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

	private static string GetCacheFolderPath(IGlobalParameters globalParameters)
	{
		string thumbnailCacheFolderPath;

		if (globalParameters.RuntimeEnvironmentType ==
			RuntimeEnvironmentType.Linux)
		{
			thumbnailCacheFolderPath = Path.Combine(
				globalParameters.UserHomePath,
				".cache",
				globalParameters.ApplicationName);
		}
		else if (globalParameters.RuntimeEnvironmentType ==
				 RuntimeEnvironmentType.LinuxFlatpak)
		{
			try
			{
				var userFlatpakConfigFolderInfo = new DirectoryInfo(
					globalParameters.UserConfigPath);

				var userFlatpakAppFolderInfo =
					userFlatpakConfigFolderInfo.Parent;
				var userFlatpakAppFolderPath =
					userFlatpakAppFolderInfo!.FullName;

				thumbnailCacheFolderPath = Path.Combine(
					userFlatpakAppFolderPath, "cache");
			}
			catch
			{
				thumbnailCacheFolderPath = globalParameters.UserConfigPath;
			}
		}
		else
		{
			thumbnailCacheFolderPath = Path.Combine(
				globalParameters.UserConfigPath,
				globalParameters.ApplicationName);
		}

		return thumbnailCacheFolderPath;
	}
}
