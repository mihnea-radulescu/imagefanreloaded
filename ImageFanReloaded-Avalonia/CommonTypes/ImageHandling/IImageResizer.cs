using Avalonia.Media;

namespace ImageFanReloaded.CommonTypes.ImageHandling;

public interface IImageResizer
{
	IImage CreateResizedImage(IImage image, ImageSize viewPortSize);
}
