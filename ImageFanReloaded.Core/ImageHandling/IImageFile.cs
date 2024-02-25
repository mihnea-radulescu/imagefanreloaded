namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageFile
{
    string ImageFileName { get; }
    
    int SizeOnDiscInKilobytes { get; }
	ImageSize ImageSize { get; }

	IImage GetImage();
	IImage GetResizedImage(ImageSize viewPortSize);

    void ReadImageDataFromDisc();
    IImage GetThumbnail();
    void DisposeImageData();

    string GetImageInfo();
}
