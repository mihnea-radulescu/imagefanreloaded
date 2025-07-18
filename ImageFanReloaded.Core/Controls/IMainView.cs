using System;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Synchronization;

namespace ImageFanReloaded.Core.Controls;

public interface IMainView
{
	IGlobalParameters? GlobalParameters { get; set; }
	IMouseCursorFactory? MouseCursorFactory { get; set; }
	ITabOptionsFactory? TabOptionsFactory { get; set; }
	IAsyncMutexFactory? AsyncMutexFactory { get; set; }

	event EventHandler<ContentTabItemCollectionEventArgs>? WindowClosing;
	event EventHandler<ContentTabItemEventArgs>? ContentTabItemAdded;
	event EventHandler<ContentTabItemEventArgs>? ContentTabItemClosed;
	event EventHandler<TabCountChangedEventArgs>? TabCountChanged;

	void AddFakeTabItem();
	void AddContentTabItem();
	void RegisterTabControlEvents();

	void Show();
}
