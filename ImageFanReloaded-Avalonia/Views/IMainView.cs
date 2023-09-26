using System;
using ImageFanReloaded.CommonTypes.CustomEventArgs;

namespace ImageFanReloaded.Views;

public interface IMainView
{
	event EventHandler<TabItemEventArgs> ContentTabItemAdded;

	void AddFakeTabItem();

	void Show();
}
