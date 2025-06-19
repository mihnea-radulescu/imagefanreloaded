namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageFile
{
    string ImageFileName { get; }
    string ImageFilePath { get; }
    
    decimal SizeOnDiscInKilobytes { get; }
	ImageSize ImageSize { get; }

	ImageInfo? ImageInfo { get; }

	IImage GetImage();
	IImage GetResizedImage(ImageSize viewPortSize);

    void ReadImageDataFromDisc();
    IImage GetThumbnail(int thumbnailSize);
    void DisposeImageData();

    string GetImageInfo(bool longFormat);
}
