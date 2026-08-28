using System;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Synchronization;

namespace ImageFanReloaded.Core.Controls;

public interface IMainView
{
	IGlobalParameters? GlobalParameters { get; set; }
	IMouseCursorFactory? MouseCursorFactory { get; set; }
	ISettingsFactory? SettingsFactory { get; set; }
	IAsyncMutexFactory? AsyncMutexFactory { get; set; }

	event EventHandler<ContentTabItemAddedEventArgs>? ContentTabItemAdded;
	event EventHandler<ContentTabItemEventArgs>? ContentTabItemClosed;
	event EventHandler<TabCountChangedEventArgs>? TabCountChanged;

	void AddFakeTabItem();

	void AddContentTabItem(
		ITabOptions? tabOptions,
		IFileSystemEntryInfo? fileSystemEntryInfoToClone,
		bool isExpandedFolderTreeViewSelectedItem);

	void CloneContentTabItem(
		ITabOptions? tabOptions,
		IFileSystemEntryInfo? fileSystemEntryInfoToClone,
		bool isExpandedFolderTreeViewSelectedItem);

	void Show();
}
