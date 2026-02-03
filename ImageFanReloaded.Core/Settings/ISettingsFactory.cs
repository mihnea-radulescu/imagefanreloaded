namespace ImageFanReloaded.Core.Settings;

public interface ISettingsFactory
{
	ITabOptions GetTabOptions();

	IThumbnailCacheOptions GetThumbnailCacheOptions();

	string GetCacheFolderPath();
}
