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

	protected override IImageFrame BuildResizedImageFrame(
		IImageFrame imageFrame,
		ImageSize resizedImageFrameSize,
		ImageQuality imageQuality)
	{
		var destinationSize = new PixelSize(
			resizedImageFrameSize.Width, resizedImageFrameSize.Height);
		var bitmapInterpolationMode = ConvertToBitmapInterpolationMode(imageQuality);

		var bitmap = imageFrame.GetBitmap();
		var resizedBitmap = bitmap.CreateScaledBitmap(destinationSize, bitmapInterpolationMode);

		var resizedImageFrame = new ImageFrame(
			resizedBitmap, resizedImageFrameSize, imageFrame.DelayUntilNextFrame);
		return resizedImageFrame;
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
