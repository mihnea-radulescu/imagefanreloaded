using System.Drawing;

namespace ImageFanReloadedWPF.CommonTypes.ImageHandling.Interface
{
    public interface IImageResizer
    {
        Image CreateThumbnail(Image image, int thumbnailSize);
        Image CreateResizedImage(Image image, Rectangle imageSize);
    }
}
