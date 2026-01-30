using System;

namespace ImageFanReloaded.Core.Caching;

public interface IDatabaseLogic
{
    void CreateDatabaseIfNotExisting();

    void DeleteDatabase();

    ThumbnailCacheEntry? GetThumbnailCacheEntry(
		string imageFilePath,
		int thumbnailSize,
		bool applyImageOrientation,
		DateTime lastModificationTime);

    void UpsertThumbnailCacheEntry(ThumbnailCacheEntry thumbnailCacheEntry);
}
