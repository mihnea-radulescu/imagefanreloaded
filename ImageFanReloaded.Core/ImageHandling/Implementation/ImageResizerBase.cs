using System.Collections.Generic;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public abstract class ImageResizerBase : IImageResizer
{
	protected ImageResizerBase(IImageResizeCalculator imageResizeCalculator)
	{
		_imageResizeCalculator = imageResizeCalculator;
	}

	public IImage CreateResizedImage(IImage image, ImageSize viewPortSize, ImageQuality imageQuality)
	{
		var imageFrames = image.ImageFrames;
		var resizedImageFrames = new List<IImageFrame>(imageFrames.Count);

		foreach (var anImageFrame in imageFrames)
		{
			var anImageFrameSize = new ImageSize(anImageFrame.Size.Width, anImageFrame.Size.Height);

			var aResizedImageFrameSize = _imageResizeCalculator
				.GetResizedImageSize(anImageFrameSize, viewPortSize);

			var aResizedImageFrame = BuildResizedImageFrame(
				anImageFrame,
				aResizedImageFrameSize,
				imageQuality);

			resizedImageFrames.Add(aResizedImageFrame);
		}

		var resizedImage = new Image(resizedImageFrames);
		return resizedImage;
	}
	
	#region Protected

	protected abstract IImageFrame BuildResizedImageFrame(
		IImageFrame imageFrame,
		ImageSize resizedImageFrameSize,
		ImageQuality imageQuality);

	#endregion
	
	#region Private
	
	private readonly IImageResizeCalculator _imageResizeCalculator;
	
	#endregion
}
