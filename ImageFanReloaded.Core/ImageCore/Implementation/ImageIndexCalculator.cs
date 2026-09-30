using System;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageCore.Implementation;

public class ImageIndexCalculator : IImageIndexCalculator
{
	public int GetIncrementedImageIndex(
		ITabOptions tabOptions, int imageCount, int imageIndex, int increment)
	{
		var shouldLoopImages = tabOptions.LoopImages;

		var incrementedImageIndex = imageIndex + increment;
		if (incrementedImageIndex < 0)
		{
			incrementedImageIndex = shouldLoopImages
				? Math.Abs(imageCount + incrementedImageIndex) % imageCount
				: 0;
		}
		else if (incrementedImageIndex >= imageCount)
		{
			incrementedImageIndex = shouldLoopImages
				? Math.Abs(incrementedImageIndex - imageCount) % imageCount
				: imageCount - 1;
		}

		return incrementedImageIndex;
	}
}
