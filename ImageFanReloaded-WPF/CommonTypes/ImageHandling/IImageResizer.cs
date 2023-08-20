using System.Drawing;

namespace ImageFanReloaded.CommonTypes.ImageHandling
{
    public interface IImageResizer
    {
        Image CreateResizedImage(Image image, ImageSize viewPortSize);
    }
}
