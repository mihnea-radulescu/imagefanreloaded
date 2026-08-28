using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class TabOptionsChangedEventArgs : ContentTabItemEventArgs
{
	public TabOptionsChangedEventArgs(
		IContentTabItem contentTabItem,
		IFileSystemEntryInfo? fileSystemEntryInfo,
		ITabOptions tabOptions,
		TabOptionChanges tabOptionChanges)
			: base(contentTabItem)
	{
		FileSystemEntryInfo = fileSystemEntryInfo;

		TabOptions = tabOptions;
		TabOptionChanges = tabOptionChanges;
	}

	public IFileSystemEntryInfo? FileSystemEntryInfo { get; }

	public ITabOptions TabOptions { get; }
	public TabOptionChanges TabOptionChanges { get; }
}
