namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public abstract class ImageResizerBase : IImageResizer
{
	protected ImageResizerBase(IImageResizeCalculator imageResizeCalculator)
	{
		_imageResizeCalculator = imageResizeCalculator;
	}

	public IImage CreateResizedImage(IImage image, ImageSize viewPortSize, ImageQuality imageQuality)
	{
		var imageSize = new ImageSize(image.Size.Width, image.Size.Height);

		var resizedImageSize = _imageResizeCalculator
			.GetResizedImageSize(imageSize, viewPortSize);

		var resizedImage = BuildResizedImage(image, resizedImageSize, imageQuality);
		return resizedImage;
	}
	
	#region Protected

	protected abstract IImage BuildResizedImage(
		IImage image, ImageSize resizedImageSize, ImageQuality imageQuality);

	#endregion
	
	#region Private
	
	private readonly IImageResizeCalculator _imageResizeCalculator;
	
	#endregion
}
