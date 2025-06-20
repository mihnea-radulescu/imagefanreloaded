namespace ImageFanReloaded.Core.Settings.Implementation;

public class TabOptionChanges : ITabOptionChanges
{
	public bool HasChangedFolderOrdering { get; set; }
	public bool HasChangedThumbnailSize { get; set; }
	public bool HasChangedRecursiveFolderBrowsing { get; set; }
	public bool HasChangedShowImageViewImageInfo { get; set; }
	public bool HasChangedPanelsSplittingRatio { get; set; }
	public bool HasChangedSlideshowInterval { get; set; }
	public bool HasChangedApplyImageOrientation { get; set; }

	public bool ShouldSaveAsDefault { get; set; }
}
