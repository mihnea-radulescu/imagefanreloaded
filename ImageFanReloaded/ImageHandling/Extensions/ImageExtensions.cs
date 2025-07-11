using Avalonia.Media.Imaging;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.ImageHandling.Extensions;

public static class ImageExtensions
{
	public static Bitmap GetBitmap(this IImage image)
		=> image.GetInstance<Bitmap>();

	public static Bitmap GetBitmap(this IImageFrame imageFrame)
		=> imageFrame.GetInstance<Bitmap>();
}
