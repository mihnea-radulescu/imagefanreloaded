using Avalonia.Media;

namespace ImageFanReloaded.CommonTypes.ImageHandling;

public interface IImageFile
{
    string FileName { get; }

	ImageSize ImageSize { get; }

	IImage GetImage();
	IImage GetResizedImage(ImageSize viewPortSize);

    void ReadImageDataFromDisc();
	IImage GetThumbnail();
    void DisposeImageData();
}
