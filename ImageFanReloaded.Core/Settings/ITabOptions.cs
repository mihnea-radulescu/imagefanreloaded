namespace ImageFanReloaded.Core.Settings;

public interface ITabOptions
{
	FileSystemEntryInfoOrdering FileSystemEntryInfoOrdering { get; set; }
	int ThumbnailSize { get; set; }
	bool RecursiveFolderBrowsing { get; set; }
	bool ShowImageViewImageInfo { get; set; }

	void SaveDefaultTabOptions();
}
