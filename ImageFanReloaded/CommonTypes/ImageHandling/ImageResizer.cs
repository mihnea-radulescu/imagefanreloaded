using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using System;
using System.Drawing;

namespace ImageFanReloaded.CommonTypes.ImageHandling
{
    public class ImageResizer
        : IImageResizer
    {
        public static readonly ImageResizer Instance = new ImageResizer();

        public Image CreateThumbnail(Image image, int thumbnailSize)
        {
            if (image == null)
                throw new ArgumentNullException("image", "The image cannot be null.");
            if (thumbnailSize <= 0)
                throw new ArgumentOutOfRangeException("thumbnailSize",
                                                      "The thumbnail size cannot be non-positive.");

            var thumbnailBounds = new Rectangle(0, 0, thumbnailSize, thumbnailSize);
            return CreateResizedImage(image, thumbnailBounds);
        }

        public Image CreateResizedImage(Image image, Rectangle imageSize)
        {
            if (image == null)
                throw new ArgumentNullException("image", "The image cannot be null.");
            if (imageSize.Width <= 0)
                throw new ArgumentOutOfRangeException("imageSize.Width",
                                                      "The width cannot be non-positive.");
            if (imageSize.Height <= 0)
                throw new ArgumentOutOfRangeException("imageSize.Height",
                                                      "The height cannot be non-positive.");
            
            var width = image.Width;
            var height = image.Height;

            if ((width > imageSize.Width) || (height > imageSize.Height))
            {
                if (width >= height)
                {
                    var aspectRatio = (float)width / (float)height;

                    width = imageSize.Width;
                    height = (int)(width / aspectRatio);

                    if (height > imageSize.Height)
                    {
                        height = imageSize.Height;
                        width = (int)(height * aspectRatio);
                    }
                }
                else
                {
                    var aspectRatio = (float)height / (float)width;

                    height = imageSize.Height;
                    width = (int)(height / aspectRatio);

                    if (width > imageSize.Width)
                    {
                        width = imageSize.Width;
                        height = (int)(width * aspectRatio);
                    }
                }
            }

            var resizedImage = new Bitmap(width, height);

            try
            {
                using (var graphics = Graphics.FromImage(resizedImage))
                    graphics.DrawImage(image, 0, 0, width, height);
            }
            catch
            {
                using (var graphics = Graphics.FromImage(resizedImage))
                    graphics.DrawImage(GlobalData.InvalidImageAsBitmap, 0, 0, width, height);
            }

            return resizedImage;
        }


        #region Private

        private ImageResizer()
        {
        }

        #endregion
    }
}
