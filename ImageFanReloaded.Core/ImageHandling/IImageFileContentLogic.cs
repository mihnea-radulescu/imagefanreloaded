using ImageFanReloaded.Core.ImageCore;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageFileContentLogic
{
	ImageData GetImageData(ImageFileData imageFileData);

	ImageData GetImageData(
		ImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation);

	void UpdateThumbnail(
		ImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation,
		IImage thumbnail);
}
