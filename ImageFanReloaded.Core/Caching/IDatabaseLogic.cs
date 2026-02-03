using System;
using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Caching;

public interface IDatabaseLogic
{
    void CreateDatabaseIfNotExisting();

	int GetThumbnailCacheSizeInMegabytes();

    Task ClearDatabase();

    ThumbnailCacheEntry? GetThumbnailCacheEntry(
		string imageFilePath,
		int thumbnailSize,
		bool applyImageOrientation,
		DateTime lastModificationTime);

    void UpsertThumbnailCacheEntry(ThumbnailCacheEntry thumbnailCacheEntry);
}
