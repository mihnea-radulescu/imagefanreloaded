using System.Linq;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public abstract class ImageResizerBase : IImageResizer
{
	protected ImageResizerBase(IImageResizeCalculator imageResizeCalculator)
	{
		_imageResizeCalculator = imageResizeCalculator;
	}

	public IImage CreateDownsizedImage(IImage image, ImageSize viewPortSize)
	{
		var downsizedImageSize = _imageResizeCalculator.GetDownsizedImageSize(image.Size, viewPortSize);

		return GetResizedImage(image, downsizedImageSize);
	}

	public IImage CreateUpsizedImage(IImage image, double scalingFactor)
	{
		var upsizedImageSize = _imageResizeCalculator.GetUpsizedImageSize(image.Size, scalingFactor);

		return GetResizedImage(image, upsizedImageSize);
	}

	#region Protected

	protected abstract IImageFrame BuildResizedImageFrame(IImageFrame imageFrame, ImageSize resizedImageFrameSize);

	#endregion

	#region Private

	private readonly IImageResizeCalculator _imageResizeCalculator;

	private IImage GetResizedImage(IImage image, ImageSize resizedImageSize)
	{
		var resizedImageFrames = image.ImageFrames
			.Select(anImageFrame => BuildResizedImageFrame(anImageFrame, resizedImageSize))
			.ToList();

		var resizedImage = new Image(resizedImageFrames);
		return resizedImage;
	}

	#endregion
}
