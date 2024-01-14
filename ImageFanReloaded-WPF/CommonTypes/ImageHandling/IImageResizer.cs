using System.Drawing;

namespace ImageFanReloaded.CommonTypes.ImageHandling
{
    public interface IImageResizer
    {
        Bitmap CreateResizedImage(Image image, ImageSize viewPortSize);
    }
}
