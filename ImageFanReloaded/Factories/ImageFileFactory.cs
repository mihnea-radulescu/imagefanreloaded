using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.Factories.Interface;

namespace ImageFanReloaded.Factories
{
    public class ImageFileFactory : IImageFileFactory
    {
        public IImageFile GetImageFile(IImageResizer imageResizer, string filePath)
            => new ImageFile(imageResizer, filePath);
    }
}
