using Avalonia.Media.Imaging;

namespace ImageFanReloaded.CommonTypes.ImageHandling;

public interface IImageResizer
{
	Bitmap CreateResizedImage(Bitmap image, ImageSize viewPortSize);
}
