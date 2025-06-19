using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Settings;

public interface ITabOptions
{
	FileSystemEntryInfoOrdering FileSystemEntryInfoOrdering { get; set; }
	ThumbnailSize ThumbnailSize { get; set; }
	bool RecursiveFolderBrowsing { get; set; }
	bool ShowImageViewImageInfo { get; set; }
	int PanelsSplittingRatio { get; set; }
	SlideshowInterval SlideshowInterval { get; set; }

	Task SaveDefaultTabOptions();
}
