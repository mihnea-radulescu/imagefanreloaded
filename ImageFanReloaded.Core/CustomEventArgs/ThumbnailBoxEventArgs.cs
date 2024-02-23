using System;
using ImageFanReloaded.Core.Controls;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class ThumbnailBoxEventArgs : EventArgs
{
	public ThumbnailBoxEventArgs(IThumbnailBox thumbnailBox)
	{
		ThumbnailBox = thumbnailBox;
	}
	
	public IThumbnailBox ThumbnailBox { get; }
}
