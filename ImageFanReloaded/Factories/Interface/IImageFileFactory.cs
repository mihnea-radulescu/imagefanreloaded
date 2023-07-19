using ImageFanReloaded.CommonTypes.ImageHandling.Interface;

namespace ImageFanReloaded.Factories.Interface
{
    public interface IImageFileFactory
    {
        IImageFile GetImageFile(IImageResizer imageResizer, string filePath);
    }
}
