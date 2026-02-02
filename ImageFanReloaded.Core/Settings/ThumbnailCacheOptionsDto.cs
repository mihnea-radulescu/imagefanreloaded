namespace ImageFanReloaded.Core.Settings;

public record ThumbnailCacheOptionsDto
{
	public bool EnableThumbnailCaching { get; set; }

	public bool ShouldClearThumbnailCache { get; set; }
}
