namespace ImageFanReloaded.Core.Settings.Implementation;

public class TabOptionChanges : ITabOptionChanges
{
	public bool HasChangedFolderOrdering { get; set; }
	public bool HasChangedThumbnailSize { get; set; }
	public bool HasChangedRecursiveFolderBrowsing { get; set; }
	public bool HasChangedShowImageViewImageInfo { get; set; }

	public TabOptionChanges()
	{
		HasChangedFolderOrdering = false;
		HasChangedThumbnailSize = false;
		HasChangedRecursiveFolderBrowsing = false;
		HasChangedShowImageViewImageInfo = false;
	}
}
