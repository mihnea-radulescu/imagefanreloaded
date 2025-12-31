using System;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageFile
{
	ImageFileData ImageFileData { get; }

	ImageSize ImageSize { get; }
	bool IsAnimatedImage { get; }
	TimeSpan AnimatedImageSlideshowDelay { get; }

	bool HasImageReadError { get; }

	IImage GetImage(bool applyImageOrientation);
	(IImage, IImage) GetImageAndResizedImage(
		ImageSize viewPortSize,
		UpsizeFullScreenImagesUpToScreenSize upsizeFullScreenImagesUpToScreenSize,
		bool applyImageOrientation);

	void ReadImageFile();

	IImage GetThumbnail(int thumbnailSize, bool applyImageOrientation);

	void RefreshImageFileData();
	string GetBasicImageInfo(bool longFormat);

	void DisposeImageFileContentStream();
}
