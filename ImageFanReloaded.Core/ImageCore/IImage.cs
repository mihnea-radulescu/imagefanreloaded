using System;
using System.Collections.Generic;

namespace ImageFanReloaded.Core.ImageCore;

public interface IImage : IImageFrame
{
	bool IsAnimated { get; }
	TimeSpan TotalImageFramesDelay { get; }

	IReadOnlyList<IImageFrame> ImageFrames { get; }

	bool DoesFitWithinViewPort(ImageSize viewPortSize);
	bool DoesFitExactlyWithinViewPort(ImageSize viewPortSize);
	bool DoesExceedViewPort(ImageSize viewPortSize);
}
