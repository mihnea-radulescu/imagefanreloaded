using System.IO;
using ImageFanReloaded.Core.Caching;
using ImageFanReloaded.Core.RuntimeEnvironment;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Caching;

public class ThumbnailCacheConfig : IThumbnailCacheConfig
{
	public ThumbnailCacheConfig(IGlobalParameters globalParameters)
	{
		_globalParameters = globalParameters;
	}

	public string GetThumbnailCacheFolderPath()
	{
		string thumbnailCacheFolderPath;

		if (_globalParameters.RuntimeEnvironmentType is
			RuntimeEnvironmentType.Linux)
		{
			thumbnailCacheFolderPath = Path.Combine(
				_globalParameters.UserHomePath,
				".cache",
				_globalParameters.ApplicationName);
		}
		else if (_globalParameters.RuntimeEnvironmentType is
				 RuntimeEnvironmentType.LinuxFlatpak)
		{
			try
			{
				var userFlatpakConfigFolderInfo = new DirectoryInfo(
					_globalParameters.UserConfigPath);

				var userFlatpakAppFolderInfo =
					userFlatpakConfigFolderInfo.Parent;
				var userFlatpakAppFolderPath =
					userFlatpakAppFolderInfo!.FullName;

				thumbnailCacheFolderPath = Path.Combine(
					userFlatpakAppFolderPath, "cache");
			}
			catch
			{
				thumbnailCacheFolderPath = _globalParameters.UserConfigPath;
			}
		}
		else
		{
			thumbnailCacheFolderPath = Path.Combine(
				_globalParameters.UserConfigPath,
				_globalParameters.ApplicationName);
		}

		return thumbnailCacheFolderPath;
	}

	private readonly IGlobalParameters _globalParameters;
}
