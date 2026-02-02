using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Settings;

public interface IThumbnailCacheOptions
{
	bool EnableThumbnailCaching { get; set; }

	bool ShouldClearThumbnailCache { get; set; }

	Task SaveThumbnailCacheOptions();
}
