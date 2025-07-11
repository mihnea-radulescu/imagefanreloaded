using System;
using ImageMagick;

namespace ImageFanReloaded.ImageHandling.Extensions;

public static class MagickImageCollectionExtensions
{
	public static void ForEach(
		this MagickImageCollection imageCollection, Action<IMagickImage> imageFrameAction)
	{
		foreach (var imageFrame in imageCollection)
		{
			imageFrameAction(imageFrame);
		}
	}
}
