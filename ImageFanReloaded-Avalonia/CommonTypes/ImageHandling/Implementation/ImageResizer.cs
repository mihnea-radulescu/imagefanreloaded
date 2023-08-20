using Avalonia.Media;
using Avalonia.Media.Imaging;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

namespace ImageFanReloaded.CommonTypes.ImageHandling.Implementation
{
    public class ImageResizer
        : IImageResizer
    {
		public ImageResizer(IImageResizeCalculator imageResizeCalculator)
        {
			_imageResizeCalculator = imageResizeCalculator;
		}

        public IImage CreateThumbnail(IImage image, int thumbnailSize)
        {
            var thumbnailDimensions = new ImageDimensions(thumbnailSize, thumbnailSize);
            return CreateResizedImage(image, thumbnailDimensions);
        }

        public IImage CreateResizedImage(IImage image, ImageDimensions imageDimensionsToResizeTo)
		{
			var imageDimensions = new ImageDimensions((int)image.Size.Width, (int)image.Size.Height);

			var resizedImageDimensions = _imageResizeCalculator
				.GetResizedImageDimensions(imageDimensions, imageDimensionsToResizeTo);

            var resizedImage = BuildResizedImage(image, resizedImageDimensions);
            return resizedImage;
        }

        #region Private

        private readonly IImageResizeCalculator _imageResizeCalculator;

        private IImage BuildResizedImage(IImage image, ImageDimensions resizedImageDimensions)
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
                            resizedImageDimensions.Width, resizedImageDimensions.Height, KnownResamplers.Lanczos3));

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
