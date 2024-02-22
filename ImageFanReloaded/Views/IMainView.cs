using System;
using ImageFanReloaded.CommonTypes.CustomEventArgs;

namespace ImageFanReloaded.Views;

public interface IMainView
{
	event EventHandler<ContentTabItemEventArgs>? ContentTabItemAdded;
	event EventHandler<ContentTabItemEventArgs>? ContentTabItemClosed;
	event EventHandler<TabCountChangedEventArgs>? TabCountChanged;

	void AddFakeTabItem();

	void Show();
}
