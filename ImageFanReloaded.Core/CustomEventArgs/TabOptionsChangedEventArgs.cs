using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class TabOptionsChangedEventArgs : ContentTabItemEventArgs
{
	public TabOptionsChangedEventArgs(
		IContentTabItem contentTabItem,
		ITabOptions tabOptions,
		TabOptionChanges tabOptionChanges)
		: base(contentTabItem)
	{
		TabOptions = tabOptions;

		TabOptionChanges = tabOptionChanges;
	}

	public ITabOptions TabOptions { get; }

	public TabOptionChanges TabOptionChanges { get; }
}
