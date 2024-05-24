using System;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class ImageViewClosingEventArgs : EventArgs
{
	public ImageViewClosingEventArgs(bool showMainView)
	{
		ShowMainView = showMainView;
	}

	public bool ShowMainView { get; }
}
