namespace ImageFanReloaded.Core.Settings.Implementation;

public class TabOptions : ITabOptions
{
	public FileSystemEntryInfoOrdering FileSystemEntryInfoOrdering { get; set; }
	public int ThumbnailSize { get; set; }
	public bool RecursiveFolderBrowsing { get; set; }
	public bool ShowImageViewImageInfo { get; set; }

	public TabOptions()
	{
		FileSystemEntryInfoOrdering = FileSystemEntryInfoOrdering.NameAscending;
		ThumbnailSize = 400;
		RecursiveFolderBrowsing = false;
		ShowImageViewImageInfo = false;
	}
}
