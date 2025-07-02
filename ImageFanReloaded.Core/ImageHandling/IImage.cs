using System.Collections.Generic;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IImage : IImageFrame
{
	bool IsAnimated { get; }

	IReadOnlyList<IImageFrame> GetImageFrames();
}
