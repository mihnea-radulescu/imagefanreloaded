using Avalonia.Media.Imaging;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.ImageHandling;

public static class ImageExtensions
{
	public static Bitmap GetBitmap(this IImage image)
		=> image.GetInstance<Bitmap>();
}
