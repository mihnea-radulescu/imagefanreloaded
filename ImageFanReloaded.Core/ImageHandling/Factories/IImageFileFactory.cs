using ImageFanReloaded.Core.ImageHandling.ImageFileData;

namespace ImageFanReloaded.Core.ImageHandling.Factories;

public interface IImageFileFactory
{
	void EnableThumbnailCaching();
	void DisableThumbnailCaching();

	IImageFile GetImageFile(IImageFileData imageFileData);
}
