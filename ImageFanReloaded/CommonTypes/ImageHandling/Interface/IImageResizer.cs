using System.Drawing;

namespace ImageFanReloaded.CommonTypes.ImageHandling.Interface
{
    public interface IImageResizer
    {
        Image CreateThumbnail(Image image, int thumbnailSize);
        Image CreateResizedImage(Image image, Rectangle imageSize);
    }
}
