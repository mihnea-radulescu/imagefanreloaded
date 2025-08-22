namespace ImageFanReloaded.Core.Settings;

public record TabOptionChanges
{
	public bool HasChangedFolderOrdering { get; set; }
	public bool HasChangedFolderOrderingDirection { get; set; }

	public bool HasChangedImageFileOrdering { get; set; }
	public bool HasChangedImageFileOrderingDirection { get; set; }

	public bool HasChangedImageViewDisplayMode { get; set; }

	public bool HasChangedThumbnailSize { get; set; }
	public bool HasChangedRecursiveFolderBrowsing { get; set; }
	public bool HasChangedShowImageViewImageInfo { get; set; }
	public bool HasChangedPanelsSplittingRatio { get; set; }
	public bool HasChangedSlideshowInterval { get; set; }
	public bool HasChangedApplyImageOrientation { get; set; }
	public bool HasChangedShowThumbnailImageFileName { get; set; }
	public bool HasChangedKeyboardScrollThumbnailIncrement { get; set; }

	public bool ShouldSaveAsDefault { get; set; }
}
