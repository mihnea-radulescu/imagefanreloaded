using System;
using ImageFanReloaded.Controls.Implementation;

namespace ImageFanReloaded.CommonTypes.CustomEventArgs;

public class ThumbnailBoxEventArgs : EventArgs
{
	public ThumbnailBoxEventArgs(ThumbnailBox thumbnailBox)
	{
		ThumbnailBox = thumbnailBox;
	}
	
	public ThumbnailBox ThumbnailBox { get; }
}
