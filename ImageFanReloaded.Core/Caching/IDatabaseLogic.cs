using System.Threading.Tasks;
using ImageFanReloaded.Core.ImageHandling.ImageFileData;

namespace ImageFanReloaded.Core.Caching;

public interface IDatabaseLogic
{
    void CreateDatabaseIfNotExisting();

	int GetThumbnailCacheSizeInMegabytes();

    Task ClearDatabase();

    ThumbnailCacheEntry? GetThumbnailCacheEntry(
		IImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation);

    void UpsertThumbnailCacheEntry(ThumbnailCacheEntry thumbnailCacheEntry);
}
