using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class TabOptionsChangedEventArgs : ContentTabItemEventArgs
{
	public TabOptionsChangedEventArgs(
		IContentTabItem contentTabItem,
		FileSystemEntryInfoOrdering fileSystemEntryInfoOrdering,
		int thumbnailSize,
		bool recursiveFolderBrowsing,
		bool showImageViewImageInfo)
		: base(contentTabItem)
	{
		FileSystemEntryInfoOrdering = fileSystemEntryInfoOrdering;
		ThumbnailSize = thumbnailSize;
		RecursiveFolderBrowsing = recursiveFolderBrowsing;
		ShowImageViewImageInfo = showImageViewImageInfo;
	}
	
	public FileSystemEntryInfoOrdering FileSystemEntryInfoOrdering { get; }
	public int ThumbnailSize { get; }
	public bool RecursiveFolderBrowsing { get; }
	public bool ShowImageViewImageInfo { get; }
}
