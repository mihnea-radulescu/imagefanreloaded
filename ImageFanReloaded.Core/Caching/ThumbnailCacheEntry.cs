using System;

namespace ImageFanReloaded.Core.Caching;

public class ThumbnailCacheEntry
{
	public required string ImageFilePath { get; init; }
	public required int ThumbnailSize { get; init; }
	public required bool ApplyImageOrientation { get; init; }

	public required DateTime LastModificationTime { get; init; }
	public required byte[] ThumbnailData { get; init; }
}
