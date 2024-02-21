using Avalonia.Media.Imaging;

namespace ImageFanReloaded.CommonTypes.ImageHandling;

public interface IImageFile
{
    string FileName { get; }
    
    int SizeOnDiscInKilobytes { get; }
	ImageSize ImageSize { get; }

	Bitmap GetImage();
	Bitmap GetResizedImage(ImageSize viewPortSize);

    void ReadImageDataFromDisc();
	Bitmap GetThumbnail();
    void DisposeImageData();

    string GetImageInfo();
}
