using System;

namespace ImageFanReloaded.CommonTypes.CustomEventArgs;

public class TabCountChangedEventArgs : EventArgs
{
	public TabCountChangedEventArgs(bool shouldDisplayTabCloseButton)
	{
		ShouldDisplayTabCloseButton = shouldDisplayTabCloseButton;
	}

	public bool ShouldDisplayTabCloseButton { get; }
}
