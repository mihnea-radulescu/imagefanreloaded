using System.Drawing;

namespace ImageFanReloaded.CommonTypes.ImageHandling
{
    public interface IImageResizer
    {
        Image CreateThumbnail(Image image, int thumbnailSize);

        Image CreateResizedImage(Image image, ImageDimensions imageDimensionsToResizeTo);
    }
}
