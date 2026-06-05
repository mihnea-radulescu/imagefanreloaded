using ImageFanReloaded.Core.ImageCore;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageDataExtractor
{
	byte[] GetImageData(IImage image);
}
