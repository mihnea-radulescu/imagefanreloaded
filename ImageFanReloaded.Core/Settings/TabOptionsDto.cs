namespace ImageFanReloaded.Core.Settings;

public record TabOptionsDto
{
	public FileSystemEntryInfoOrdering FolderOrdering { get; set; }
	public FileSystemEntryInfoOrderingDirection FolderOrderingDirection
		{ get; set; }

	public FileSystemEntryInfoOrdering ImageFileOrdering { get; set; }
	public FileSystemEntryInfoOrderingDirection ImageFileOrderingDirection
		{ get; set; }

	public ImageViewDisplayMode ImageViewDisplayMode { get; set; }

	public int ThumbnailSize { get; set; }

	public bool RecursiveFolderBrowsing { get; set; }
	public bool GlobalOrderingForRecursiveFolderBrowsing { get; set; }

	public bool ShowImageViewImageInfo { get; set; }
	public int PanelsSplittingRatio { get; set; }
	public decimal SlideshowInterval { get; set; }
	public bool ApplyImageOrientation { get; set; }
	public bool ShowThumbnailImageFileName { get; set; }
	public int KeyboardScrollThumbnailIncrement { get; set; }
	public UpsizeFullScreenImagesUpToScreenSize
		UpsizeFullScreenImagesUpToScreenSize { get; set; }
}
