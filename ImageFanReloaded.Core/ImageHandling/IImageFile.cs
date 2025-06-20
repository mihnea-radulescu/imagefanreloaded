namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageFile
{
	ImageFileData ImageFileData { get; }

	ImageSize ImageSize { get; }

	bool HasReadImageError { get; }

	IImage GetImage(bool applyImageOrientation);
	IImage GetResizedImage(ImageSize viewPortSize, bool applyImageOrientation);
	void ReadImageDataFromDisc(bool applyImageOrientation);

	IImage GetThumbnail(int thumbnailSize);
	void DisposeImageData();

	string GetImageInfo(bool longFormat);
}
