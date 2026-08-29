using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class ContentTabItemChangedEventArgs : ContentTabItemEventArgs
{
	public ContentTabItemChangedEventArgs(
		IContentTabItem contentTabItem,
		IFileSystemEntryInfo fileSystemEntryInfo,
		bool hasChangedFolderTree,
		bool hasChangedFolderContent,
		bool hasChangedFolderInfo,
		bool hasChangedPanelsSplittingRatio)
			: base(contentTabItem)
	{
		FileSystemEntryInfo = fileSystemEntryInfo;

		HasChangedFolderTree = hasChangedFolderTree;
		HasChangedFolderContent = hasChangedFolderContent;
		HasChangedFolderInfo = hasChangedFolderInfo;
		HasChangedPanelsSplittingRatio = hasChangedPanelsSplittingRatio;
	}

	public IFileSystemEntryInfo FileSystemEntryInfo { get; }

	public bool HasChangedFolderTree { get; }
	public bool HasChangedFolderContent { get; }
	public bool HasChangedFolderInfo { get; }
	public bool HasChangedPanelsSplittingRatio { get; }
}
