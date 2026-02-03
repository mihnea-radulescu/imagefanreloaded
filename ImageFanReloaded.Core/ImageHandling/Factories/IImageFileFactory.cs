namespace ImageFanReloaded.Core.ImageHandling.Factories;

public interface IImageFileFactory
{
	void EnableThumbnailCaching();
	void DisableThumbnailCaching();

	IImageFile GetImageFile(ImageFileData imageFileData);
}
