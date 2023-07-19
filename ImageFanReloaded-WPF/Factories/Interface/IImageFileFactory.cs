using ImageFanReloadedWPF.CommonTypes.ImageHandling.Interface;

namespace ImageFanReloadedWPF.Factories.Interface
{
    public interface IImageFileFactory
    {
        IImageFile GetImageFile(IImageResizer imageResizer, string filePath);
    }
}
