using Avalonia.Media;

namespace ImageFanReloaded.CommonTypes.ImageHandling
{
    public interface IImageResizer
    {
        IImage CreateThumbnail(IImage image, int thumbnailSize);

		IImage CreateResizedImage(IImage image, ImageDimensions imageDimensionsToResizeTo);
    }
}
