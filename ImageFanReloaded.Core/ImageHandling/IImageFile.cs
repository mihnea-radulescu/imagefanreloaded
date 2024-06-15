namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageFile
{
    string ImageFileName { get; }
    string ImageFilePath { get; }
    
    decimal SizeOnDiscInKilobytes { get; }
	ImageSize ImageSize { get; }

	IImage GetImage();
	IImage GetResizedImage(ImageSize viewPortSize);

    void ReadImageDataFromDisc();
    IImage GetThumbnail();
    void DisposeImageData();

    string GetImageInfo(bool longFormat);
}
