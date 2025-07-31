using System;
using ImageFanReloaded.Core.Controls;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class ImageChangedEventArgs : EventArgs
{
	public ImageChangedEventArgs(IImageView imageView, int increment)
	{
		ImageView = imageView;
		Increment = increment;
	}

	public IImageView ImageView { get; }
	public int Increment { get; }
}
