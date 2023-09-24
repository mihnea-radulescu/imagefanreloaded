using ImageFanReloaded.CommonTypes.ImageHandling;

namespace ImageFanReloaded.Factories;

public interface IImageFileFactory
{
    IImageFile GetImageFile(string filePath);
}
