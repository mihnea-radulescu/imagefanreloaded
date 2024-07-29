using System.IO;
using Microsoft.Extensions.Configuration;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Settings;

public class DefaultAppSettings : IAppSettings
{
	public DefaultAppSettings()
	{
		ThumbnailSizeInPixels = DefaultThumbnailSizeInPixels;
		
		try
		{
			if (Path.Exists(SettingsFilePath))
			{
				var settings = BuildConfiguration(SettingsFilePath);
				var appSettings = settings.GetSection(AppSettingsSectionName);

				var thumbnailSizeInPixelsFromAppSettings = int.Parse(appSettings[nameof(ThumbnailSizeInPixels)]!);

				if (thumbnailSizeInPixelsFromAppSettings
						is >= ThumbnailSizeInPixelsLowerBound and <= ThumbnailSizeInPixelsUpperBound)
				{
					ThumbnailSizeInPixels = thumbnailSizeInPixelsFromAppSettings;
				}
			}
		}
		catch
		{
		}
	}
	
	public int ThumbnailSizeInPixels { get; }
	
	#region Private

	private const string SettingsFilePath = "AppSettings.json";
	private const string AppSettingsSectionName = "AppSettings";

	private const int DefaultThumbnailSizeInPixels = 250;
	private const int ThumbnailSizeInPixelsLowerBound = 50;
	private const int ThumbnailSizeInPixelsUpperBound = 500;
	
	private static IConfigurationRoot BuildConfiguration(string jsonConfigFile)
		=> new ConfigurationBuilder().AddJsonFile(jsonConfigFile).Build();
	
	#endregion
}
