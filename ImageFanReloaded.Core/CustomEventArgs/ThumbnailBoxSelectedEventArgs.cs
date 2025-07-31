using System;
using ImageFanReloaded.Core.Controls;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class ThumbnailBoxSelectedEventArgs : EventArgs
{
	public ThumbnailBoxSelectedEventArgs(IThumbnailBox thumbnailBox)
	{
		ThumbnailBox = thumbnailBox;
	}

	public IThumbnailBox ThumbnailBox { get; }
}
