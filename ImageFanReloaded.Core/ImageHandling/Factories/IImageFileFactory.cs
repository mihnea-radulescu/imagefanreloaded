namespace ImageFanReloaded.Core.ImageHandling.Factories;

public interface IImageFileFactory
{
    IImageFile GetImageFile(ImageFileData imageFileData);
}
