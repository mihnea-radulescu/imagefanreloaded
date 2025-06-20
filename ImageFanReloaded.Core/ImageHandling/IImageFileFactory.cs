namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageFileFactory
{
    IImageFile GetImageFile(ImageFileData imageFileData);
}
