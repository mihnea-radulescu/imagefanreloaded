using System;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Mouse;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class ThumbnailBoxClickedEventArgs : EventArgs
{
	public ThumbnailBoxClickedEventArgs(
		IThumbnailBox thumbnailBox,
		ClickType clickType)
	{
		ThumbnailBox = thumbnailBox;
		
		ClickType = clickType;
	}
	
	public IThumbnailBox ThumbnailBox { get; }

	public ClickType ClickType { get; }
}
