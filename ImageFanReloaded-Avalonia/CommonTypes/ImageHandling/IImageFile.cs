using Avalonia.Media;

namespace ImageFanReloaded.CommonTypes.ImageHandling
{
    public interface IImageFile
    {
        string FileName { get; }

        IImage GetImage();
		IImage GetResizedImage(ImageSize imageSize);

        void ReadThumbnailInputFromDisc();
		IImage GetThumbnail();
        void DisposeThumbnailInput();
    }
}
