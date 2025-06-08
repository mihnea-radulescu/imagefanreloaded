using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class TabOptionsChangedEventArgs : ContentTabItemEventArgs
{
	public TabOptionsChangedEventArgs(
		IContentTabItem contentTabItem,
		ITabOptions tabOptions)
		: base(contentTabItem)
	{
		TabOptions = tabOptions;
	}
	
	public ITabOptions TabOptions { get; }
}
