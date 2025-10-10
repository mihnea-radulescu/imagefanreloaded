using System;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageFile
{
	StaticImageFileData StaticImageFileData { get; }
	TransientImageFileData TransientImageFileData { get; }

	ImageSize ImageSize { get; }
	bool IsAnimatedImage { get; }
	TimeSpan AnimatedImageSlideshowDelay { get; }

	bool HasImageReadError { get; }

	IImage GetImage(bool applyImageOrientation);
	(IImage, IImage) GetImageAndResizedImage(ImageSize viewPortSize, bool applyImageOrientation);
	void ReadImageFile();

	IImage GetThumbnail(int thumbnailSize, bool applyImageOrientation);

	void RefreshTransientImageFileData();
	string GetBasicImageInfo(bool longFormat);

	void DisposeImageFileContentStream();
}
