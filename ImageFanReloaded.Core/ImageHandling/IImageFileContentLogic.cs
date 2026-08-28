using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.ImageHandling.ImageFileData;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageFileContentLogic
{
	ImageData GetImageData(IImageFileData imageFileData);

	ImageData GetImageData(
		IImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation);

	void UpdateThumbnail(
		IImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation,
		IImage thumbnail);
}
