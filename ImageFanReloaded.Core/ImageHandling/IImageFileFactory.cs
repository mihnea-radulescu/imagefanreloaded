namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageFileFactory
{
    IImageFile GetImageFile(string fileName, string filePath, decimal sizeOnDiscInKilobytes);
}
