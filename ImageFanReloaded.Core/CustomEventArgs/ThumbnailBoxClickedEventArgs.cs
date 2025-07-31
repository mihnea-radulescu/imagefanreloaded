using System;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Mouse;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class ThumbnailBoxClickedEventArgs : EventArgs
{
	public ThumbnailBoxClickedEventArgs(
		IThumbnailBox thumbnailBox,
		MouseClickType mouseClickType)
	{
		ThumbnailBox = thumbnailBox;

		MouseClickType = mouseClickType;
	}

	public IThumbnailBox ThumbnailBox { get; }

	public MouseClickType MouseClickType { get; }
}
