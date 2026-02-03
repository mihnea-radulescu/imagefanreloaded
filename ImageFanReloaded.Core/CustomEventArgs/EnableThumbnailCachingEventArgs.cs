using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class EnableThumbnailCachingEventArgs
	: ThumbnailCacheOptionsChangedEventArgs
{
	public EnableThumbnailCachingEventArgs(
		IThumbnailCacheOptions thumbnailCacheOptions)
	{
		ThumbnailCacheOptions = thumbnailCacheOptions;
	}

	public IThumbnailCacheOptions ThumbnailCacheOptions { get; }
}
