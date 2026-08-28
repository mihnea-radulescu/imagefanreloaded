using System;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class FolderChangedEventArgs : EventArgs
{
	public FolderChangedEventArgs(
		IContentTabItem contentTabItem,
		IFileSystemEntryInfo fileSystemEntryInfo)
	{
		ContentTabItem = contentTabItem;
		FileSystemEntryInfo = fileSystemEntryInfo;
	}

	public IContentTabItem ContentTabItem { get; }
	public IFileSystemEntryInfo FileSystemEntryInfo { get; }
}
