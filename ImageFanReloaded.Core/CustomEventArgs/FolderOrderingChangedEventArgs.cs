using System;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class FolderOrderingChangedEventArgs : EventArgs
{
	public FolderOrderingChangedEventArgs(
		IContentTabItem contentTabItem,
		IFileSystemEntryInfo fileSystemEntryInfoToClone)
	{
		ContentTabItem = contentTabItem;
		FileSystemEntryInfoToClone = fileSystemEntryInfoToClone;
	}

	public IContentTabItem ContentTabItem { get; }
	public IFileSystemEntryInfo FileSystemEntryInfoToClone { get; }
}
