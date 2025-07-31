using System;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class ImageSelectedEventArgs : EventArgs
{
	public ImageSelectedEventArgs(
		IContentTabItem contentTabItem,
		IImageFile imageFile)
	{
		ContentTabItem = contentTabItem;

		ImageFile = imageFile;
	}

	public IContentTabItem ContentTabItem { get; }

	public IImageFile ImageFile { get; }
}
