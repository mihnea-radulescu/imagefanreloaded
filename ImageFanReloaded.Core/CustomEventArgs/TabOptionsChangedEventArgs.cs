using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class TabOptionsChangedEventArgs : ContentTabItemEventArgs
{
	public TabOptionsChangedEventArgs(
		IContentTabItem contentTabItem,
		FileSystemEntryInfo? fileSystemEntryInfo,
		ITabOptions tabOptions,
		TabOptionChanges tabOptionChanges)
			: base(contentTabItem)
	{
		FileSystemEntryInfo = fileSystemEntryInfo;

		TabOptions = tabOptions;
		TabOptionChanges = tabOptionChanges;
	}

	public FileSystemEntryInfo? FileSystemEntryInfo { get; }

	public ITabOptions TabOptions { get; }
	public TabOptionChanges TabOptionChanges { get; }
}
