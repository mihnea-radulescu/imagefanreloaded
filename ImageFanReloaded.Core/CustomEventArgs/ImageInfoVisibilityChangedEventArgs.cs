using System;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class ImageInfoVisibilityChangedEventArgs : EventArgs
{
	public ImageInfoVisibilityChangedEventArgs(bool isVisible)
	{
		IsVisible = isVisible;
	}
	
	public bool IsVisible { get; }
}
