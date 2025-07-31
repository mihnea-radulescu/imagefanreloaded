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
	IImage GetResizedImage(ImageSize viewPortSize, bool applyImageOrientation);
	void ReadImageDataFromDisc(bool applyImageOrientation);

	IImage GetThumbnail(int thumbnailSize);

	void RefreshTransientImageFileData();
	string GetBasicImageInfo(bool longFormat);

	void DisposeImageData();
}
