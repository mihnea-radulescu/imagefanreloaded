using ImageFanReloaded.Core.Controls;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class ClearThumbnailCacheEventArgs
	: ThumbnailCacheOptionsChangedEventArgs
{
	public ClearThumbnailCacheEventArgs(
		IThumbnailCacheOptionsView thumbnailCacheOptionsView)
	{
		ThumbnailCacheOptionsView = thumbnailCacheOptionsView;
	}

	public IThumbnailCacheOptionsView ThumbnailCacheOptionsView { get; }
}
