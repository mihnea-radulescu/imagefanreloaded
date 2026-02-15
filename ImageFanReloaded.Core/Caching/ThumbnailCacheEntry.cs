using System;

namespace ImageFanReloaded.Core.Caching;

public class ThumbnailCacheEntry
{
	public required string FilePath { get; init; }
	public required int FileSizeInBytes { get; init; }
	public required DateTime FileLastModificationTime { get; init; }

	public required int ThumbnailSize { get; init; }
	public required bool ApplyImageOrientation { get; init; }

	public required byte[] ThumbnailData { get; init; }
}
