using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class TabOptionsChangedEventArgs : ContentTabItemEventArgs
{
	public TabOptionsChangedEventArgs(
		IContentTabItem contentTabItem,
		ITabOptionChanges tabOptionChanges)
		: base(contentTabItem)
	{
		TabOptionChanges = tabOptionChanges;
	}
	
	public ITabOptionChanges TabOptionChanges { get; }
}
