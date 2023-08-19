using System.Drawing;

namespace ImageFanReloaded.CommonTypes.ImageHandling.Implementation
{
    public class ImageResizer
        : IImageResizer
    {
        public Image CreateThumbnail(Image image, int thumbnailSize)
        {
            var thumbnailBounds = new Rectangle(0, 0, thumbnailSize, thumbnailSize);
            return CreateResizedImage(image, thumbnailBounds);
        }

        public Image CreateResizedImage(Image image, Rectangle imageSize)
        {
            var resizedImage = GetResizedImage(image, imageSize);

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

        private Image GetResizedImage(Image image, Rectangle imageSize)
        {
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
            return resizedImage;
        }

        #endregion
    }
}
