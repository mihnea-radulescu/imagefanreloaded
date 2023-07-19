using ImageFanReloadedWPF.CommonTypes.ImageHandling;
using ImageFanReloadedWPF.CommonTypes.ImageHandling.Interface;
using ImageFanReloadedWPF.Factories.Interface;

namespace ImageFanReloadedWPF.Factories
{
    public class ImageFileFactory : IImageFileFactory
    {
        public IImageFile GetImageFile(IImageResizer imageResizer, string filePath)
            => new ImageFile(imageResizer, filePath);
    }
}
