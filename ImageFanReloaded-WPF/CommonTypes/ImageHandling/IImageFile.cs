using System.Windows.Media;

namespace ImageFanReloaded.CommonTypes.ImageHandling
{
    public interface IImageFile
    {
        string FileName { get; }

        ImageSource GetImage();
		ImageSource GetResizedImage(ImageSize imageSize);

        void ReadThumbnailInputFromDisc();
        ImageSource GetThumbnail();
        void DisposeThumbnailInput();
    }
}
