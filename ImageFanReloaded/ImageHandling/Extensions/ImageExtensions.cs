using Avalonia.Media.Imaging;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.ImageHandling.Extensions;

public static class ImageExtensions
{
	extension(IImage image)
	{
		public Bitmap Bitmap => image.GetInstance<Bitmap>();
	}

	extension(IImageFrame imageFrame)
	{
		public Bitmap Bitmap => imageFrame.GetInstance<Bitmap>();
	}
}
