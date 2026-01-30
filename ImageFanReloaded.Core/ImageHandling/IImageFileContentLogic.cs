namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageFileContentLogic
{
	ImageData GetImageData(string imageFilePath, bool applyImageOrientation);
	ImageData GetImageData(
		string imageFilePath, int thumbnailSize, bool applyImageOrientation);

	void UpdateThumbnail(
		string imageFilePath,
		int thumbnailSize,
		bool applyImageOrientation,
		IImage thumbnail);
}
