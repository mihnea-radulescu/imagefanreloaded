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

        public Bitmap CreateResizedImage(Image image, ImageSize viewPortSize)
		{
            var resizedImage = GetResizedImage(image, viewPortSize);

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

		private Bitmap GetResizedImage(Image image, ImageSize viewPortSize)
		{
			var imageSize = new ImageSize(image.Width, image.Height);

			var resizedImageSize = _imageResizeCalculator
                .GetResizedImageSize(imageSize, viewPortSize);

			var resizedImage = new Bitmap(resizedImageSize.Width, resizedImageSize.Height);
			return resizedImage;
		}

		#endregion
	}
}
