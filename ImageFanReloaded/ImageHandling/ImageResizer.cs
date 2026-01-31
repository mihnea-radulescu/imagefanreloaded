using Avalonia;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.ImageHandling.Extensions;

namespace ImageFanReloaded.ImageHandling;

public class ImageResizer : ImageResizerBase
{
	public ImageResizer(IImageResizeCalculator imageResizeCalculator)
		: base(imageResizeCalculator)
	{
	}

	protected override IImageFrame BuildResizedImageFrame(
		IImageFrame imageFrame, ImageSize resizedImageFrameSize)
	{
		var destinationSize = new PixelSize(
			resizedImageFrameSize.Width, resizedImageFrameSize.Height);

		var bitmap = imageFrame.Bitmap;
		var resizedBitmap = bitmap.CreateScaledBitmap(destinationSize);

		var resizedImageFrame = new ImageFrame(
			resizedBitmap,
			resizedImageFrameSize,
			imageFrame.DelayUntilNextFrame);

		return resizedImageFrame;
	}
}
