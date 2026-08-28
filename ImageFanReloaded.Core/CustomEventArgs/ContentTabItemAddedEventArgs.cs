using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class ContentTabItemAddedEventArgs : ContentTabItemEventArgs
{
	public ContentTabItemAddedEventArgs(
		IContentTabItem contentTabItem,
		IFileSystemEntryInfo? fileSystemEntryInfoToClone,
		bool isExpandedFolderTreeViewSelectedItem)
			: base(contentTabItem)
	{
		FileSystemEntryInfoToClone = fileSystemEntryInfoToClone;

		IsExpandedFolderTreeViewSelectedItem =
			isExpandedFolderTreeViewSelectedItem;
	}

	public IFileSystemEntryInfo? FileSystemEntryInfoToClone { get; }
	public bool IsExpandedFolderTreeViewSelectedItem { get; }

	public bool ShouldCloneActiveTab => FileSystemEntryInfoToClone is not null;
}
