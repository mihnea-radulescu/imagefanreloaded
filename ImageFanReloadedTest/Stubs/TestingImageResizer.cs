using ImageFanReloaded;
using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using System.Drawing;

namespace ImageFanReloadedTest.Stubs
{
    public class TestingImageResizer
        : IImageResizer
    {
        public Image CreateThumbnail(Image image, int thumbnailSize)
        {
            var thumbnailBounds = new Rectangle(0, 0, thumbnailSize, thumbnailSize);
            return CreateResizedImage(image, thumbnailBounds);
        }

        public Image CreateResizedImage(Image image, Rectangle imageSize)
        {
            var width = imageSize.Width;
            var height = imageSize.Height;

            var resizedImage = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(resizedImage))
                graphics.DrawImage(GlobalData.InvalidImageAsBitmap, 0, 0, width, height);

            return resizedImage;
        }
    }
}
