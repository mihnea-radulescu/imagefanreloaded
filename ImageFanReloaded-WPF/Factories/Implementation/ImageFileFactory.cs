using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.ImageHandling.Implementation;

namespace ImageFanReloaded.Factories.Implementation
{
    public class ImageFileFactory
        : IImageFileFactory
    {
        public IImageFile GetImageFile(IImageResizer imageResizer, string filePath)
            => new ImageFile(imageResizer, filePath);
    }
}
