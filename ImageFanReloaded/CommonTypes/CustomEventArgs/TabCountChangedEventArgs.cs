using System;

namespace ImageFanReloaded.CommonTypes.CustomEventArgs;

public class TabCountChangedEventArgs : EventArgs
{
	public TabCountChangedEventArgs(bool showTabCloseButton)
	{
		ShowTabCloseButton = showTabCloseButton;
	}

	public bool ShowTabCloseButton { get; }
}
