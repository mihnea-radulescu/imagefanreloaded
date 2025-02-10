using Avalonia;
using Avalonia.Media.Imaging;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;

namespace ImageFanReloaded.ImageHandling;

public class ImageResizer : ImageResizerBase
{
	public ImageResizer(IImageResizeCalculator imageResizeCalculator)
		: base(imageResizeCalculator)
	{
	}
	
	#region Protected

	protected override IImage BuildResizedImage(
		IImage image, ImageSize resizedImageSize, ImageQuality imageQuality)
	{
		var destinationSize = new PixelSize(resizedImageSize.Width, resizedImageSize.Height);
		var bitmapInterpolationMode = ConvertToBitmapInterpolationMode(imageQuality);

		var bitmap = image.GetBitmap();
		var resizedBitmap = bitmap.CreateScaledBitmap(destinationSize, bitmapInterpolationMode);
		
		var resizedImage = new Image(resizedBitmap, resizedImageSize);
		return resizedImage;
	}

	#endregion

	#region Private

	private static BitmapInterpolationMode ConvertToBitmapInterpolationMode(ImageQuality imageQuality)
	{
		return imageQuality switch
		{
			ImageQuality.Medium => BitmapInterpolationMode.MediumQuality,
			_ => BitmapInterpolationMode.HighQuality,
		};
	}

	#endregion
}
