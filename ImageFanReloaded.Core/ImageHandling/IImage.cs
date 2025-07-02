using System.Collections.Generic;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IImage : IImageFrame
{
	IReadOnlyList<IImageFrame> GetImageFrames();
}
