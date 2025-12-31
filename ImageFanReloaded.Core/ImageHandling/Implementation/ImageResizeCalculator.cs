namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ImageResizeCalculator : IImageResizeCalculator
{
	public ImageSize GetDownsizedImageSize(ImageSize imageSize, ImageSize viewPortSize)
	{
		if (imageSize.Width <= viewPortSize.Width &&
			imageSize.Height <= viewPortSize.Height)
		{
			return imageSize;
		}

		var downsizedWidth = imageSize.Width;
		var downsizedHeight = imageSize.Height;

		if (downsizedWidth >= downsizedHeight)
		{
			var aspectRatio = downsizedWidth / (double)downsizedHeight;

			downsizedWidth = viewPortSize.Width;
			downsizedHeight = (int)(downsizedWidth / aspectRatio);

			if (downsizedHeight > viewPortSize.Height)
			{
				downsizedHeight = viewPortSize.Height;
				downsizedWidth = (int)(downsizedHeight * aspectRatio);
			}
		}
		else
		{
			var aspectRatio = downsizedHeight / (double)downsizedWidth;

			downsizedHeight = viewPortSize.Height;
			downsizedWidth = (int)(downsizedHeight / aspectRatio);

			if (downsizedWidth > viewPortSize.Width)
			{
				downsizedWidth = viewPortSize.Width;
				downsizedHeight = (int)(downsizedWidth * aspectRatio);
			}
		}

		var downsizedImageSize = new ImageSize(downsizedWidth, downsizedHeight);
		return downsizedImageSize;
	}

	public ImageSize GetUpsizedImageSize(ImageSize imageSize, double scalingFactor)
		=> new ImageSize(imageSize.Width * scalingFactor, imageSize.Height * scalingFactor);
}
