using System;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.DiscAccess;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class FolderChangedEventArgs : EventArgs
{
	public FolderChangedEventArgs(
		IContentTabItem contentTabItem, FileSystemEntryInfo fileSystemEntryInfo)
	{
		ContentTabItem = contentTabItem;
		FileSystemEntryInfo = fileSystemEntryInfo;
	}

	public IContentTabItem ContentTabItem { get; }
	public FileSystemEntryInfo FileSystemEntryInfo { get; }
}
