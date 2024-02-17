namespace ImageFanReloaded.CommonTypes.ImageHandling.Implementation;

public class ImageResizeCalculator : IImageResizeCalculator
{
	public ImageSize GetResizedImageSize(
		ImageSize imageSize, ImageSize viewPortSize)
	{
		if (imageSize.Width <= viewPortSize.Width &&
			imageSize.Height <= viewPortSize.Height)
		{
			return imageSize;
		}

		var resizedWidth = imageSize.Width;
		var resizedHeight = imageSize.Height;

		if (resizedWidth >= resizedHeight)
		{
			var aspectRatio = (float)resizedWidth / (float)resizedHeight;

			resizedWidth = viewPortSize.Width;
			resizedHeight = (int)(resizedWidth / aspectRatio);

			if (resizedHeight > viewPortSize.Height)
			{
				resizedHeight = viewPortSize.Height;
				resizedWidth = (int)(resizedHeight * aspectRatio);
			}
		}
		else
		{
			var aspectRatio = (float)resizedHeight / (float)resizedWidth;

			resizedHeight = viewPortSize.Height;
			resizedWidth = (int)(resizedHeight / aspectRatio);

			if (resizedWidth > viewPortSize.Width)
			{
				resizedWidth = viewPortSize.Width;
				resizedHeight = (int)(resizedWidth * aspectRatio);
			}
		}

		var resizedImageSize = new ImageSize(resizedWidth, resizedHeight);
		return resizedImageSize;
	}
}
