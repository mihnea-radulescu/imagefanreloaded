using System;
using System.Collections.Generic;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IImage : IImageFrame
{
	bool IsAnimated { get; }
	TimeSpan TotalImageFramesDelay { get; }

	IReadOnlyList<IImageFrame> ImageFrames { get; }

	bool DoesFitWithinViewPort(ImageSize viewPortSize);
	double GetMaxUpscalingFactorToViewPort(ImageSize viewPortSize);
}
