using System;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Synchronization;

namespace ImageFanReloaded.Core.Controls;

public interface IMainView
{
	IGlobalParameters? GlobalParameters { get; set; }
	ITabOptionsFactory? TabOptionsFactory { get; set; }

	IFolderChangedMutexFactory? FolderChangedMutexFactory { get; set; }
	
	event EventHandler<ContentTabItemEventArgs>? ContentTabItemAdded;
	event EventHandler<ContentTabItemEventArgs>? ContentTabItemClosed;
	event EventHandler<TabCountChangedEventArgs>? TabCountChanged;

	void AddFakeTabItem();
	void AddContentTabItem();
	void RegisterTabControlEvents();

	void Show();
}
