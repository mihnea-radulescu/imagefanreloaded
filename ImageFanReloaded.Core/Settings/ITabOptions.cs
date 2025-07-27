using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Settings;

public interface ITabOptions
{
	FileSystemEntryInfoOrdering FolderOrdering { get; set; }
	FileSystemEntryInfoOrderingDirection FolderOrderingDirection { get; set; }

	FileSystemEntryInfoOrdering ImageFileOrdering { get; set; }
	FileSystemEntryInfoOrderingDirection ImageFileOrderingDirection { get; set; }

	ThumbnailSize ThumbnailSize { get; set; }
	bool RecursiveFolderBrowsing { get; set; }
	bool ShowImageViewImageInfo { get; set; }
	int PanelsSplittingRatio { get; set; }
	SlideshowInterval SlideshowInterval { get; set; }
	bool ApplyImageOrientation { get; set; }
	bool ShowThumbnailImageFileName { get; set; }

	Task SaveDefaultTabOptions();
}
