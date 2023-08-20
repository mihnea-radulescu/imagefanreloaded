using System;
using System.IO;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace ImageFanReloaded.CommonTypes.ImageHandling.Implementation
{
    public class ImageResizer
        : IImageResizer
    {
		public ImageResizer(IImageResizeCalculator imageResizeCalculator)
        {
			_imageResizeCalculator = imageResizeCalculator;
		}

        public IImage CreateResizedImage(IImage image, ImageSize viewPortSize)
		{
			var imageSize = new ImageSize(image.Size.Width, image.Size.Height);

			var resizedImageSize = _imageResizeCalculator
				.GetResizedImageSize(imageSize, viewPortSize);

            var resizedImage = BuildResizedImage(image, resizedImageSize);
            return resizedImage;
        }

        #region Private

        private readonly IImageResizeCalculator _imageResizeCalculator;

        private IImage BuildResizedImage(IImage image, ImageSize resizedImageSize)
        {
            var bitmap = image as Bitmap;

			if (bitmap == null)
			{
				throw new InvalidOperationException($"Input image must be a {nameof(Bitmap)} object.");
			}

            using (Stream stream = new MemoryStream())
			{
                bitmap.Save(stream);
                stream.Position = 0;

				using (SixLabors.ImageSharp.Image loadedImage = SixLabors.ImageSharp.Image.Load(stream))
				{
					loadedImage.Mutate(context =>
                        context.Resize(
                            resizedImageSize.Width, resizedImageSize.Height, KnownResamplers.Lanczos3));

                    using (Stream saveStream = new MemoryStream())
                    {
						loadedImage.Save(saveStream, new JpegEncoder());
                        saveStream.Position = 0;

                        var resizedImage = new Bitmap(saveStream);
                        return resizedImage;
					}
				}
			}
		}

        #endregion
    }
}
