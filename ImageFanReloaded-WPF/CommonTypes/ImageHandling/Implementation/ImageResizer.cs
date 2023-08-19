using System.Drawing;

namespace ImageFanReloaded.CommonTypes.ImageHandling.Implementation
{
    public class ImageResizer
        : IImageResizer
    {
		public ImageResizer(IImageResizeCalculator imageResizeCalculator)
        {
			_imageResizeCalculator = imageResizeCalculator;
		}

        public Image CreateThumbnail(Image image, int thumbnailSize)
        {
            var thumbnailDimensions = new ImageDimensions(thumbnailSize, thumbnailSize);
            return CreateResizedImage(image, thumbnailDimensions);
        }

        public Image CreateResizedImage(Image image, ImageDimensions imageDimensionsToResizeTo)
		{
            var resizedImage = GetResizedImage(image, imageDimensionsToResizeTo);

            try
            {
                using (var graphics = Graphics.FromImage(resizedImage))
                {
                    graphics.DrawImage(image, 0, 0, resizedImage.Width, resizedImage.Height);
                }
            }
            catch
            {
                using (var graphics = Graphics.FromImage(resizedImage))
                {
                    graphics.DrawImage(GlobalData.InvalidImageAsBitmap, 0, 0, resizedImage.Width, resizedImage.Height);
                }
            }

            return resizedImage;
        }

		#region Private

		private readonly IImageResizeCalculator _imageResizeCalculator;

		private Image GetResizedImage(Image image, ImageDimensions imageDimensionsToResizeTo)
		{
			var imageDimensions = new ImageDimensions(image.Width, image.Height);

			var resizedImageDimensions = _imageResizeCalculator
                .GetResizedImageDimensions(imageDimensions, imageDimensionsToResizeTo);

			var resizedImage = new Bitmap(resizedImageDimensions.Width, resizedImageDimensions.Height);
			return resizedImage;
		}

		#endregion
	}
}
