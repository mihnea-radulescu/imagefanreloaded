using System;
using System.Threading.Tasks;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface IThumbnailCacheOptionsView
{
	IGlobalParameters? GlobalParameters { get; set; }
	IThumbnailCacheOptions? ThumbnailCacheOptions { get; set; }

	int? ThumbnailCacheSizeInMegabytes { get; set; }

	event EventHandler<EnableThumbnailCachingEventArgs>?
		EnableThumbnailCachingChanged;
	event EventHandler<ClearThumbnailCacheEventArgs>? ClearThumbnailCacheSelected;

	void PopulateThumbnailCacheOptions();

	Task ShowDialog(IMainView owner);
}
