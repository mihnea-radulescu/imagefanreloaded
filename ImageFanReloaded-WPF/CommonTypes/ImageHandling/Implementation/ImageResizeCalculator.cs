namespace ImageFanReloaded.CommonTypes.ImageHandling.Implementation
{
	public class ImageResizeCalculator
		: IImageResizeCalculator
	{
		public ImageDimensions GetResizedImageDimensions(
			ImageDimensions imageDimensions, ImageDimensions imageDimensionsToResizeTo)
		{
			var resizedWidth = imageDimensions.Width;
			var resizedHeight = imageDimensions.Height;

			if ((resizedWidth > imageDimensionsToResizeTo.Width) || (resizedHeight > imageDimensionsToResizeTo.Height))
			{
				if (resizedWidth >= resizedHeight)
				{
					var aspectRatio = (float)resizedWidth / (float)resizedHeight;

					resizedWidth = imageDimensionsToResizeTo.Width;
					resizedHeight = (int)(resizedWidth / aspectRatio);

					if (resizedHeight > imageDimensionsToResizeTo.Height)
					{
						resizedHeight = imageDimensionsToResizeTo.Height;
						resizedWidth = (int)(resizedHeight * aspectRatio);
					}
				}
				else
				{
					var aspectRatio = (float)resizedHeight / (float)resizedWidth;

					resizedHeight = imageDimensionsToResizeTo.Height;
					resizedWidth = (int)(resizedHeight / aspectRatio);

					if (resizedWidth > imageDimensionsToResizeTo.Width)
					{
						resizedWidth = imageDimensionsToResizeTo.Width;
						resizedHeight = (int)(resizedWidth * aspectRatio);
					}
				}
			}

			var resizedImageDimensions = new ImageDimensions(resizedWidth, resizedHeight);
			return resizedImageDimensions;
		}
	}
}
