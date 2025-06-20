namespace ImageFanReloaded.Core.Settings;

public interface ITabOptionChanges
{
	bool HasChangedFolderOrdering { get; set; }
	bool HasChangedThumbnailSize { get; set; }
	bool HasChangedRecursiveFolderBrowsing { get; set; }
	bool HasChangedShowImageViewImageInfo { get; set; }
	bool HasChangedPanelsSplittingRatio { get; set; }
	bool HasChangedSlideshowInterval { get; set; }
	bool HasChangedApplyImageOrientation { get; set; }

	bool ShouldSaveAsDefault { get; set; }
}
